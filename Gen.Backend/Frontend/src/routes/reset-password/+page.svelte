<script>
    import { onMount } from "svelte";
    import { fade } from "svelte/transition";
    import {Button} from "$lib/components/ui/button/index.js";

    let email = "";
    let password = "";
    let confirmPassword = "";
    let token = "";
    let success = false;
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

            const responseData = await response.json();

            if (response.ok) {
                success = true;
            } else {
                errorMessage = responseData.message || "Password reset failed.";
                errorList = responseData.errors || [];
            }
        } catch (error) {
            errorMessage = "An error occurred. Please try again.";
        } finally {
            isLoading = false;
        }
    }
</script>

<main class="min-h-screen bg-gradient-to-r from-blue-50 to-indigo-50 flex items-center justify-center">
    <div class="w-full max-w-4xl mx-auto px-4">
        <div class="text-center bg-white p-8 rounded-lg shadow-lg opacity-0 animate-fade-in">

            <!-- Success-View -->
            {#if success}
                
                <!-- Success-Symbol -->
                <div class="flex justify-center mb-6 opacity-0 animate-fade-in delay-100">
                    <div class="w-16 h-16 bg-green-500 rounded-full flex items-center justify-center">
                        <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                        </svg>
                    </div>
                </div>
                <h1 class="text-4xl font-bold text-gray-800 mb-4 opacity-0 animate-fade-in delay-100">Password Reset Successful</h1>
                <p class="text-xl text-gray-600 opacity-0 animate-fade-in delay-100">You can now log in with your new password.</p>

            <!-- Reset-View --> 
            {:else}
                
                <!-- Lock-Symbol -->
                <div class="flex justify-center mb-6 opacity-0 animate-fade-in delay-100">
                    <div class="w-16 h-16 bg-blue-500 rounded-full flex items-center justify-center">
                        <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path>
                        </svg>
                    </div>
                </div>

                <h1 class="text-4xl font-bold text-gray-800 mb-4 opacity-0 animate-fade-in delay-100">Reset Your Password</h1>
                <p class="text-xl text-gray-600 mb-8 opacity-0 animate-fade-in delay-100">Enter your new password below.</p>

                <!-- Form -->
                <form class="space-y-6" on:submit|preventDefault={resetPassword}>
                    
                    <input type="password" bind:value={password} placeholder="New Password" required class="w-full px-4 py-3 border border-gray-300 rounded-lg opacity-0 animate-fade-in delay-200" />
                    <input type="password" bind:value={confirmPassword} placeholder="Confirm Password" required class="w-full px-4 py-3 border border-gray-300 rounded-lg opacity-0 animate-fade-in delay-200" />

                    <!-- Errors -->
                    {#if errorMessage}
                        <p class="text-red-500 mt-2 mb-2">{errorMessage}</p>
                    {/if}
                    {#if errorList.length > 0}
                        <ul class="text-red-500 list-disc list-inside">
                            {#each errorList as err}
                                <li>{err}</li>
                            {/each}
                        </ul>
                    {/if}
                    <Button class="w-full">Test</Button>
                    <button type="submit" class="w-full px-8 py-3 text-white bg-blue-600 rounded-lg hover:bg-blue-700 opacity-0 animate-fade-in delay-300" disabled={isLoading}>{isLoading ? "Resetting Password..." : "Reset Password"}</button>
                </form>
            {/if}
        </div>
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