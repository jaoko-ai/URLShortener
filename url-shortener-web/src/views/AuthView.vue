<script setup lang="ts">
import { computed, ref } from "vue";
import { RouterLink, useRoute, useRouter } from "vue-router";
import { useAuthStore } from "../stores/auth";

const auth = useAuthStore();
const route = useRoute();
const router = useRouter();

const mode = computed(() => (route.meta.mode === "register" ? "register" : "login"));
const username = ref("");
const email = ref("");
const password = ref("");
const localError = ref("");

async function onSubmit() {
  localError.value = "";
  if (!email.value.trim() || !password.value) {
    localError.value = "Email and password are required.";
    return;
  }
  if (mode.value === "register" && !username.value.trim()) {
    localError.value = "Pick a username.";
    return;
  }

  try {
    if (mode.value === "register") {
      await auth.register({
        username: username.value.trim(),
        email: email.value.trim(),
        password: password.value,
      });
    } else {
      await auth.login({
        email: email.value.trim(),
        password: password.value,
      });
    }
    await router.push("/");
  } catch {
    // Store holds the API error.
  }
}
</script>

<template>
  <div class="mx-auto max-w-md">
    <section class="rounded-3xl border border-line bg-card p-7 shadow-[0_24px_50px_-32px_rgba(28,25,20,0.45)]">
      <p class="text-xs uppercase tracking-[0.22em] text-ember">
        {{ mode === "register" ? "Create account" : "Welcome back" }}
      </p>
      <h1 class="mt-2 font-display text-4xl">
        {{ mode === "register" ? "Sign up" : "Log in" }}
      </h1>
      <p class="mt-2 text-muted">
        Auth is wired through Pinia so the backend can drop in JWT login later.
      </p>

      <form class="mt-6 space-y-4" @submit.prevent="onSubmit">
        <label v-if="mode === 'register'" class="block">
          <span class="mb-1.5 block text-sm text-muted">Username</span>
          <input
            v-model="username"
            type="text"
            autocomplete="username"
            class="w-full rounded-2xl border border-line bg-paper px-4 py-3 outline-none ring-ember/30 focus:ring-2"
          />
        </label>
        <label class="block">
          <span class="mb-1.5 block text-sm text-muted">Email</span>
          <input
            v-model="email"
            type="email"
            autocomplete="email"
            class="w-full rounded-2xl border border-line bg-paper px-4 py-3 outline-none ring-ember/30 focus:ring-2"
          />
        </label>
        <label class="block">
          <span class="mb-1.5 block text-sm text-muted">Password</span>
          <input
            v-model="password"
            type="password"
            :autocomplete="mode === 'register' ? 'new-password' : 'current-password'"
            class="w-full rounded-2xl border border-line bg-paper px-4 py-3 outline-none ring-ember/30 focus:ring-2"
          />
        </label>

        <p v-if="localError || auth.error" class="text-sm text-ember-dark">
          {{ localError || auth.error }}
        </p>

        <button
          type="submit"
          class="w-full rounded-full bg-pine py-3 text-card hover:bg-moss disabled:opacity-60"
          :disabled="auth.loading"
        >
          {{ auth.loading ? "Working…" : mode === "register" ? "Create account" : "Log in" }}
        </button>
      </form>

      <p class="mt-5 text-center text-sm text-muted">
        <template v-if="mode === 'register'">
          Already have an account?
          <RouterLink to="/login" class="text-pine underline underline-offset-4">Log in</RouterLink>
        </template>
        <template v-else>
          New here?
          <RouterLink to="/register" class="text-pine underline underline-offset-4">Sign up</RouterLink>
        </template>
      </p>
    </section>
  </div>
</template>
