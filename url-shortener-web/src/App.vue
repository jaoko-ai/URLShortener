<script setup lang="ts">
import AppHeader from "./components/AppHeader.vue";
</script>

<template>
  <div class="flex min-h-screen flex-col">
    <AppHeader />
    <main class="mx-auto w-full max-w-6xl flex-1 px-5 py-8 sm:py-12">
      <RouterView />
    </main>
    <footer class="border-t border-line/70 px-5 py-6 text-center text-sm text-muted">
      Shortpath · frontend template for a learning ASP.NET API
    </footer>
  </div>
</template>
