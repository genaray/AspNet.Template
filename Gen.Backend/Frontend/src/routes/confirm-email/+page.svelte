<script>
    import { onMount } from "svelte";

    let message = "Confirming your email...";
    let success = false;
    let loading = true; 

    onMount(async () => {
        
        // Get params
        const params = new URLSearchParams(window.location.search);
        const email = params.get("email");
        const token = params.get("token");

        if (!email || !token) {
            message = "Invalid confirmation link.";
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

<!-- Screen with card -->
<main class="min-h-screen bg-gradient-to-r from-blue-50 to-indigo-50 flex items-center justify-center">
    <div class="w-full max-w-4xl mx-auto px-4">
        <div class="text-center bg-white p-8 rounded-lg shadow-lg opacity-0 animate-fade-in">

            <!-- Loading anim or result -->
            {#if loading}
                <div class="flex flex-col items-center">
                    <svg class="animate-spin h-10 w-10 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <circle cx="12" cy="12" r="10" stroke-width="4"></circle>
                        <path stroke-linecap="round" stroke-width="4" d="M12 2v4m0 12v4m10-10h-4m-12 0H2"></path>
                    </svg>
                    <p class="text-xl text-gray-600 mt-4">Confirming your email...</p>
                </div>
            {:else}
                <div class="flex items-center justify-center mb-6 opacity-0 animate-fade-in delay-100">
                    <div class="w-12 h-12 rounded-full flex items-center justify-center" class:bg-green-500={success} class:bg-red-500={!success}>
                        
                        <!--Success or failure icon-->
                        {#if success}
                            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                            </svg>
                        {:else}
                            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                            </svg>
                        {/if}
                    </div>

                    <!--Success or failure title-->
                    <h1 class="text-4xl font-bold text-gray-800 ml-4">
                        {success ? "Email Confirmed!" : "Confirmation Failed"}
                    </h1>
                </div>

                <!--Display message-->
                <p class="text-xl text-gray-600 mb-8 opacity-0 animate-fade-in delay-200">
                    {message}
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
</style>