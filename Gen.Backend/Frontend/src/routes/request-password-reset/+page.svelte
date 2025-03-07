<script>
    
    let email = "";
    let isSubmitted = false;
    let isLoading = false;
    let errorMessage = "";

    async function requestPasswordReset(event) {
        
        event.preventDefault();
        isLoading = true;
        errorMessage = "";

        try {
            const response = await fetch("http://localhost:8080/api/Authenticate/request-password-reset", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "An error occurred");
            }

            isSubmitted = true;
        } catch (error) {
            errorMessage = error.message;
        } finally {
            isLoading = false;
        }
    }
</script>

<main class="min-h-screen bg-gradient-to-r from-blue-50 to-indigo-50 flex items-center justify-center">
    <div class="w-full max-w-4xl mx-auto px-4">
        <div class="text-center bg-white p-8 rounded-lg shadow-lg opacity-0 animate-fade-in">

            <!--Success or lock icon-->
            <div class="flex justify-center mb-6 opacity-0 animate-fade-in delay-100">
                <div class="w-16 h-16 rounded-full flex items-center justify-center" class:bg-green-500={isSubmitted} class:bg-blue-500={!isSubmitted}>
                    {#if isSubmitted}
                        <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                        </svg>
                    {:else}
                        <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path>
                        </svg>
                    {/if}
                </div>
            </div>

            <!-- Titel -->
            <h1 class="text-4xl font-bold text-gray-800 mb-4 opacity-0 animate-fade-in delay-100">
                {isSubmitted ? "Email has been send!" : "Reset your Password"}
            </h1>

            <!-- Beschreibung -->
            {#if !isSubmitted}
                <p class="text-xl text-gray-600 mb-8 opacity-0 animate-fade-in delay-100">
                    Enter your email address below, and we'll send you a link to reset your password.
                </p>

                <!-- E-Mail-Formular -->
                <form on:submit={requestPasswordReset} class="space-y-6 opacity-0 animate-fade-in delay-200">
                    <div class="text-left">
                        <label for="email" class="block text-sm font-medium text-gray-700 mb-2">Email Address</label>
                        <input
                                type="email"
                                id="email"
                                name="email"
                                bind:value={email}
                                placeholder="Enter your email address"
                                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                required
                        />
                    </div>

                    <!-- Submit-Button -->
                    <button
                            type="submit"
                            class="w-full px-8 py-3 text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition transform hover:scale-105"
                            disabled={isLoading}
                    >
                        {isLoading ? "Sending..." : "Send Reset Link"}
                    </button>
                </form>

                {#if errorMessage}
                    <p class="text-red-500 mt-4">{errorMessage}</p>
                {/if}
            {:else}
                <!-- Erfolgsmeldung -->
                <p class="text-lg text-green-600 font-medium">
                    If an account with that email exists, we have sent a reset link!
                </p>
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