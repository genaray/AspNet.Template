
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

console.log("TEST")

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		port: 5173, // Entwicklungsserver-Port
	}
});
