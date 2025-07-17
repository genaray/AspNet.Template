# terraform init
# terraform plan
# terraform apply
# terraform destroy

# To set the aws credentials, use this in shell before executing 
# export AWS_ACCESS_KEY_ID=AKIA…DEINKEY
# export AWS_SECRET_ACCESS_KEY=abcd…DEINSECRET

# Terraform setup
terraform {
  required_version = ">= 1.4.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"    # AWS provider plugin
      version = "~> 5.0"           # Use version 5.x
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"  # Kubernetes provider to interact with cluster
      version = "~> 2.18"               # Use version 2.x
    }
    kustomization = {
      source  = "kbst/kustomization"    # Community provider to apply kustomize overlays
      version = "~> 0.9.0"              # Use version 0.9.x
    }
  }
}

# Locals
locals {
  raw_ids = data.kustomization_build.app.ids

  # Ignore those ids
  ignore_ids = [
    "_/Pod/default/release-name-grafana-test",
    "_/ServiceAccount/default/release-name-grafana-test"
  ]
  
  # IDs without ignored ids
  app_ids = [
    for id in local.raw_ids :
    id
    if !contains(local.ignore_ids, id)
  ]

  # Services to deploy
  svc_ids = [
    for id in local.app_ids :
    id
    if can(regex("^Service/", id))
  ]

  kubeconfig = templatefile("${path.module}/kubeconfig.tpl", {
    cluster_name = module.eks.cluster_name
    endpoint     = module.eks.cluster_endpoint
    ca_data      = module.eks.cluster_certificate_authority_data
    token        = data.aws_eks_cluster_auth.cluster.token
  })
}

# Configure AWS provider
provider "aws" {
  region = var.aws_region   # Use region from variables.tf (default eu-central-1)
  access_key = "#"
  secret_key = "#"
}

# Create or reference a default VPC for EKS
data "aws_vpc" "default" {
  default = true           # Select the default VPC in the region
}

# Fetch all subnets in the default VPC
data "aws_subnets" "private" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

# EKS module: provisions managed control plane and node group
module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = ">= 20.0.0"
  
  # Cluster metadata
  cluster_name    = var.cluster_name
  cluster_version = "1.27"

  # Networking
  vpc_id     = data.aws_vpc.default.id
  subnet_ids = data.aws_subnets.private.ids

  # Acess cluster from this machine
  cluster_endpoint_public_access        = true
  cluster_endpoint_public_access_cidrs = ["0.0.0.0/0"]
  cluster_endpoint_private_access       = false
  
  # Control‐plane access & authentication
  # Automatically grant your AWS caller identity cluster-admin
  enable_cluster_creator_admin_permissions = true

  # Managed Node Group defaults & definitions 
  eks_managed_node_group_defaults = {
    instance_types = ["t3.small"]
    disk_size      = 20
  }

  eks_managed_node_groups = {
    free_tier = {
      desired_size = 2
      min_size     = 2
      max_size     = 4
    }
  }

  # Tags for everything
  tags = {
    Environment = "dev"
    Terraform   = "true"
  }
}

# Retrieve cluster details for Kubernetes provider
data "aws_eks_cluster" "cluster" {
  name = module.eks.cluster_name
  depends_on = [module.eks]  # After successfull eks setup
}

data "aws_eks_cluster_auth" "cluster" {
  name = module.eks.cluster_name
  depends_on = [module.eks] # After successfull eks setup
}

# Configure Kubernetes provider using EKS credentials
provider "kubernetes" {
  host                   = module.eks.cluster_endpoint
  cluster_ca_certificate = base64decode(module.eks.cluster_certificate_authority_data)
  token                  = data.aws_eks_cluster_auth.cluster.token
}

# Kustomization provider, kubeconfig by aws eks to acess the kubernetes cluster by kustomization
provider "kustomization" {
  kubeconfig_raw = local.kubeconfig
}

# Builds kustomization file, assigns ids to each service (as a string)
data "kustomization_build" "app" {
  path = "${path.module}/../k8s/"
}

# Apply Kustomize 
resource "kustomization_resource" "app" {
  for_each = toset(local.app_ids) # Iterating over all ids returned by kustomization_build
  manifest = data.kustomization_build.app.manifests[each.value] # Manifest json for each id
  wait = true                                   # Wait until resources are ready
  depends_on = [module.eks]                     # Ensure cluster is up first
}

# Get all service-names
data "kubernetes_service" "all" {
  for_each = toset(local.svc_ids)
  metadata {
    name      = split("/", each.value)[1] #     # id is e.g. "Service/authentication-service"
    namespace = "default"
  }
}

# Output nodeports 
output "services_nodeports" {
  value = {
    for id, svc in data.kubernetes_service.all :
    # split und index 1 ist der eigentliche Service‑Name
    split("/", id)[1] => svc.spec[0].ports[0].node_port
  }
}

# Output ips
output "cluster_ips" {
  value = {
    for id, svc in data.kubernetes_service.all :
    split("/", id)[1] => svc.spec[0].cluster_ip
  }
}

# Output kubeconfig file to connect to the cluster
output "kubeconfig_raw" {
  sensitive = true
  value = local.kubeconfig
}