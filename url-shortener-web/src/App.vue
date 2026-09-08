<script setup lang="ts">
import HelloWorld from "./components/HelloWorld.vue";
</script>

<template>
  <h1 class="text-3xl font-bold underline">Hello world!</h1>
  <HelloWorld />

  <p><strong>Current route path:</strong> {{ $route.name }}</p>
  <nav>
    <RouterLink to="/">Go to Home</RouterLink>
    <RouterLink to="/about">Go to About</RouterLink>
  </nav>
  <main>
    <RouterView />
  </main>
</template>
