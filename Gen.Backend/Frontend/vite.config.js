import tailwindcss from "@tailwindcss/vite";
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

console.log("TEST")

export default defineConfig({
	plugins: [sveltekit(), tailwindcss()],
	server: {
		port: 5173, // Entwicklungsserver-Port
	}
});
