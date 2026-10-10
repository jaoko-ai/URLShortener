<script setup lang="ts">
import { RouterLink, useRouter } from "vue-router";
import { useAuthStore } from "../stores/auth";

const auth = useAuthStore();
const router = useRouter();

async function onLogout() {
  await auth.logout();
  await router.push("/");
}
</script>

<template>
  <header class="border-b border-line/80 bg-card/80 backdrop-blur-sm sticky top-0 z-20">
    <div class="mx-auto flex max-w-6xl items-center justify-between gap-6 px-5 py-4">
      <RouterLink to="/" class="flex items-baseline gap-2">
        <span class="font-display text-2xl tracking-tight">Shortpath</span>
        <span class="hidden text-xs uppercase tracking-[0.18em] text-muted sm:inline">
          url shortener
        </span>
      </RouterLink>

      <nav class="flex items-center gap-1 text-sm sm:gap-2">
        <RouterLink
          to="/"
          class="rounded-full px-3 py-1.5 text-muted transition hover:bg-paper hover:text-ink"
        >
          Studio
        </RouterLink>
        <RouterLink
          to="/about"
          class="rounded-full px-3 py-1.5 text-muted transition hover:bg-paper hover:text-ink"
        >
          About
        </RouterLink>

        <template v-if="auth.isAuthenticated">
          <span class="hidden max-w-40 truncate px-2 text-muted sm:inline">
            {{ auth.user?.username }}
          </span>
          <button
            type="button"
            class="rounded-full border border-line px-3 py-1.5 hover:bg-paper"
            @click="onLogout"
          >
            Log out
          </button>
        </template>
        <template v-else>
          <RouterLink
            to="/login"
            class="rounded-full px-3 py-1.5 text-muted transition hover:bg-paper hover:text-ink"
          >
            Log in
          </RouterLink>
          <RouterLink
            to="/register"
            class="rounded-full bg-pine px-3 py-1.5 text-card transition hover:bg-moss"
          >
            Sign up
          </RouterLink>
        </template>
      </nav>
    </div>
  </header>
</template>
