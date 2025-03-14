<script>
    import { onMount } from "svelte";
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
    let password = "";
    let confirmPassword = "";
    let token = "";
    let isSuccess = false;
    let errorMessage = "";
    let errorList = [];
    let isLoading = false;

    onMount(() => {
        const params = new URLSearchParams(window.location.search);
        email = params.get("email");
        token = params.get("token");

        if (!email || !token) {
            errorMessage = "Invalid reset link.";
        }
    });

    async function resetPassword() {
        if (password !== confirmPassword) {
            errorMessage = "Passwords do not match.";
            return;
        }

        isLoading = true;
        errorMessage = "";

        try {
            const response = await fetch("http://localhost:8080/api/Authenticate/reset-password", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, token, newPassword: password })
            });

            if (response.ok) {
                isSuccess = true;
            } else {
                const responseData = await response.json();
                errorMessage = responseData.message || "Password reset failed.";
                errorList = responseData.errors || [];
            }
        } catch (error) {
            errorMessage = `An error occurred: ${error.message}. Please try again.`;
        } finally {
            isLoading = false;
        }
    }
</script>

<main class="min-h-screen bg-background flex items-center justify-center">
    <div class="opacity-0 animate-fade-in">

        <Card class="w-full max-w-4xl mx-auto p-6">

            <!-- Card Header with icon fading in -->
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
                        {isSuccess ? "Password Reset Successful" : "Reset your Password"}
                    </CardTitle>
                    <CardDescription class="text-xl">
                        {isSuccess ? "You can now log in with your new password." : "Enter your new password below, and we'll update your account."}
                    </CardDescription>
                </CardHeader>
            </div>

            <!-- Card content with input form -->
            {#if !isSuccess}
                <div class="opacity-0 animate-fade-in delay-200">
                    <CardContent>

                        <!-- Input form -->
                        <form on:submit|preventDefault={resetPassword} class="space-y-6">
                            
                            <input type="password" bind:value={password} placeholder="New Password" required class="w-full px-4 py-3 border border-gray-300 rounded-lg opacity-0 animate-fade-in delay-200" />
                            <input type="password" bind:value={confirmPassword} placeholder="Confirm Password" required class="w-full px-4 py-3 border border-gray-300 rounded-lg opacity-0 animate-fade-in delay-200" />
                            
                            <!-- Errors -->
                            {#if errorMessage}
                                <p class="text-red-500">{errorMessage}</p>
                            {/if}
                            {#if errorList.length > 0}
                                <ul class="text-red-500 list-disc list-inside">
                                    {#each errorList as err}
                                        <li>{err}</li>
                                    {/each}
                                </ul>
                            {/if}

                            <!-- Send -->
                            <Button type="submit" class="w-full" disabled={isLoading}>
                                {#if isLoading}
                                    <Loader2 class="mr-2 animate-spin" /> Resetting password...
                                {:else}
                                    Reset password
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