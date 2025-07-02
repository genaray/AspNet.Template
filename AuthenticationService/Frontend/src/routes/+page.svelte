<script>
    // Hier kannst du ggf. Logik für die Seite hinzufügen
</script>

<main class="min-h-screen bg-gradient-to-r from-blue-50 to-indigo-50 flex items-center justify-center">
    <div class="w-full max-w-6xl mx-auto px-4">
        <div class="text-left">
            <h1 class="text-5xl font-bold text-gray-800 mb-6 opacity-0 animate-slide-in">
                Welcome to Placeholder
            </h1>
            <p class="text-xl text-gray-600 mb-8 max-w-2xl opacity-0 animate-slide-in delay-100">
                Thank you for visiting our project. We are excited to have you! Explore our platform and discover the tools and resources we offer to help you succeed.
            </p>
            <a href="#" class="inline-block px-8 py-3 text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition transform hover:scale-105 opacity-0 animate-slide-in delay-200">
                Get Started
            </a>
        </div>
    </div>
</main>

<style>
    @keyframes slide-in {
        from {
            transform: translateX(-100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }

    .animate-slide-in {
        animation: slide-in 0.8s ease-out forwards;
    }

    .delay-100 {
        animation-delay: 0.2s;
    }

    .delay-200 {
        animation-delay: 0.4s;
    }
</style>