import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(({ mode }) => {
    // Read VITE_API_PROXY_TARGET from .env files. During development,
    // Vite forwards /api requests to the running ASP.NET Core backend.
    const env = loadEnv(mode, process.cwd(), "");
    const apiTarget = env.VITE_API_PROXY_TARGET || "http://localhost:5000";

    return {
        plugins: [react()],
        server: {
            proxy: {
                "/api": {
                    target: apiTarget,
                    changeOrigin: true,
                    secure: false
                }
            }
        },
        build: {
            // Build directly into the ASP.NET Core wwwroot folder.
            // emptyOutDir:false prevents Vite from deleting other files in wwwroot.
            outDir: "../wwwroot",
            emptyOutDir: false,
            assetsDir: "assets"
        }
    };
});
