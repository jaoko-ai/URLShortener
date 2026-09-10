<script setup lang="ts"></script>

<template>
  <header
    class="flex justify-around items-center sticky top-0 h-20 p-1 shadow-xl mb-5 hover:shadow-lg bg-white duration-150 transition-shadow ease-in"
  >
    <h1 class="text-2xl font-mono">
      <RouterLink to="/">Url shortener</RouterLink>
    </h1>
    <nav>
      <RouterLink to="/about">About</RouterLink>

      <RouterLink to="/login">Login</RouterLink>
    </nav>
  </header>
  <main class="h-screen w-screen flex justify-center align-middle">
    <div>
      <RouterView />
    </div>
  </main>
</template>
