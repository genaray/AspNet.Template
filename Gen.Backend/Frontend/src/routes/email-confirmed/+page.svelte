<script>
    // Hier kannst du ggf. Logik für die Seite hinzufügen
</script>

<main class="min-h-screen bg-gradient-to-r from-blue-50 to-indigo-50 flex items-center justify-center">
    <div class="w-full max-w-4xl mx-auto px-4">
        <div class="text-center bg-white p-8 rounded-lg shadow-lg opacity-0 animate-fade-in">
            
            <!-- Titel mit Haken-Symbol daneben -->
            <div class="flex items-center justify-center mb-6 opacity-0 animate-fade-in delay-100">
                <!-- Haken-Symbol -->
                <div class="w-12 h-12 bg-green-500 rounded-full flex items-center justify-center">
                    <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                    </svg>
                </div>
                <!-- Titel -->
                <h1 class="text-4xl font-bold text-gray-800 ml-4">
                    Email Confirmed!
                </h1>
            </div>

            <!-- Beschreibung -->
            <p class="text-xl text-gray-600 mb-8 opacity-0 animate-fade-in delay-200">
                Your email address has been successfully confirmed. Thank you for verifying your account!
                You can now login to the application and close this page.
            </p>
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