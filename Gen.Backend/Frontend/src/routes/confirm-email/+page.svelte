<script>
    import { onMount } from "svelte";
    import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "$lib/components/ui/card";
    import {Check, X, Loader2, CheckCircle, XCircle} from "lucide-svelte";

    let message = "Confirming your email...";
    let success = false;
    let loading = true;

    onMount(async () => {
        
        // Get params
        const params = new URLSearchParams(window.location.search);
        const email = params.get("email");
        const token = params.get("token");

        if (!email || !token) {
            message = "Invalid confirmation link. There was an issue with your link, it is probably corrupted or out of date. Please contact the support.";
            loading = false;
            return;
        }

        // Post confirm and display message
        try {
            const response = await fetch(`http://localhost:8080/api/Authenticate/confirm-email?email=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`, {
                method: "POST"
            });

            if (response.ok) {
                message = "Your email has been successfully confirmed!\nWe’re thrilled to have you join our community. You may now login and close this page!";
                success = true;
            } else {
                const errorData = await response.text();
                message = `Confirming Email failed: ${errorData}. Please contact the support.`;
            }
        } catch (error) {
            message = "An error occurred while confirming your email. Please try again later.";
        } finally {
            loading = false; // Finish loading
        }
    });
</script>

<main class="min-h-screen bg-background flex items-center justify-center">
    
    <!-- Card with fade-in animation -->
    <div class="opacity-0 animate-fade-in">
        <Card class="w-full max-w-4xl px-4 mx-4">

            <!-- Header fading in -->
            <div class="opacity-0 animate-fade-in delay-100">
                <CardHeader class="text-center">

                    <!-- Icon -->
                    <div class="flex items-center justify-center">
                        {#if loading}
                            <Loader2 class="h-16 w-16 text-blue-500 animate-spin" />
                        {:else if success}
                            <CheckCircle class="h-16 w-16 text-green-500" />
                        {:else}
                            <XCircle class="h-16 w-16 text-red-500" />
                        {/if}
                    </div>
    
                    <!-- Title -->
                    <CardTitle class="text-4xl font-bold text-gray-800">
                        {success ? "Email Confirmed!" : "Confirmation Failed"}
                    </CardTitle>
                </CardHeader>
            </div>

            <!-- Content with fade-in animation -->
            <div class="opacity-0 animate-fade-in delay-200">
                <CardContent class="text-center">
                    <p class="text-xl text-gray-600">
                        {message}
                    </p>
                </CardContent>
            </div>
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
        animation: fade-in 0.5s ease-out forwards;
    }

    .delay-200 {
        animation-delay: 0.2s;
    }
</style>