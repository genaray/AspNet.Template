<script>

    import {
        Card,
        CardContent,
        CardDescription,
        CardFooter,
        CardHeader,
        CardTitle
    } from "$lib/components/ui/card/index.js";
    import {CheckCircle, Loader2, Lock, XCircle} from "lucide-svelte";
    import {Input} from "$lib/components/ui/input/index.js";
    import {Button} from "$lib/components/ui/button/index.js";

    let email = "";
    let isSuccess = false;
    let isLoading = false;
    let errorMessage = "";

    async function requestPasswordReset(event) {
        
        event.preventDefault();
        isLoading = true;
        errorMessage = "";

        try {
            
            // Send request
            const response = await fetch("http://localhost:8080/api/Authenticate/request-password-reset", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email })
            });

            // Extract ok or error
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "An error occurred");
            }

            isSuccess = true;
        } catch (error) {
            errorMessage = `Requesting password reset failed: ${error.message}. Please contact the support.`;
        } finally {
            isLoading = false;
        }
    }
</script>

<main class="min-h-screen flex items-center justify-center bg-background">

    <div class="opacity-0 animate-fade-in">
        <Card class="w-full max-w-4xl mx-auto p-6">
    
            <!-- Header fading in -->
            <div class="opacity-0 animate-fade-in delay-100">
                <CardHeader class="text-center">
    
                    <!-- Icon -->
                    <div class="flex items-center justify-center">
                        {#if isLoading}
                            <Loader2 class="h-16 w-16 text-blue-500 animate-spin" />
                        {:else if !isLoading && !isSuccess}
                            <Lock class="h-16 w-16" />
                        {:else if isSuccess}
                            <CheckCircle class="h-16 w-16 text-green-500" />
                        {:else}
                            <XCircle class="h-16 w-16 text-red-500" />
                        {/if}
                    </div>
    
                    <!-- Title -->
                    <CardTitle class="text-4xl font-bold text-gray-800">
                        {isSuccess ? "Email has been sent!" : "Reset your Password"}
                    </CardTitle>
                    <CardDescription class="text-xl"> 
                        {isSuccess ? "If an account with that email exists, we have sent a reset link!" : "Enter your email address below, and we'll send you a link to reset your password."}
                    </CardDescription>
                </CardHeader>
            </div>
    
            <!-- Content fading in-->
            {#if !isSuccess}
                <div class="opacity-0 animate-fade-in delay-200">
                    <CardContent>
                        
                            <!-- Input form -->
                            <form on:submit={requestPasswordReset} class="space-y-6">
             
                                <Input type="email" id="email" name="email" bind:value={email} placeholder="Enter your email address" required />
                                
                                {#if errorMessage}
                                    <div class="text-red-500">{errorMessage}</div>
                                {/if}

                                <!-- Send -->
                                <Button type="submit" class="w-full" disabled={isLoading}>
                                    {#if isLoading}
                                        <Loader2 class="mr-2 animate-spin" /> Sending...
                                    {:else}
                                        Send Reset Link
                                    {/if}
                                </Button>
                            </form>
                    </CardContent>
                </div>
            {/if}
        </Card>
    </div>
</main>

<style>
    @keyframes fade-in {
        from {
            opacity: 0;
        }
        to {
            opacity: 1;
        }
    }

    .animate-fade-in {
        animation: fade-in 1s ease-out forwards;
    }

    .delay-100 {
        animation-delay: 0.2s;
    }

    .delay-200 {
        animation-delay: 0.4s;
    }

    .delay-300 {
        animation-delay: 0.6s;
    }
</style>