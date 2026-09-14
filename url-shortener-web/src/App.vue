<script setup lang="ts"></script>

<template>
  <header
    class="flex justify-around items-center sticky top-0 h-20 p-1 shadow-xl mb-5 hover:shadow-lg bg-white duration-150 transition-shadow ease-in"
  >
    <h1 class="text-2xl font-mono">
      <RouterLink to="/">Url shortener</RouterLink>
    </h1>
    <nav>
      <RouterLink
        class="hover:bg-gray-100 transition-colors ease-linear w-3 h-3 p-3"
        to="/auth"
        >Login</RouterLink
      >

      <RouterLink
        class="w-3 h-3 p-3 m-3 hover:bg-gray-100 transition-colors ease-in"
        to="/about"
        >About</RouterLink
      >
    </nav>
  </header>
  <main class="h-screen w-screen flex justify-center align-middle">
    <div>
      <RouterView />
    </div>
  </main>
</template>
