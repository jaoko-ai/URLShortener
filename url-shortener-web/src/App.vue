<script setup lang="ts"></script>

<template>
  <header class="sticky top-0 border m-5 p-1 shadow shadow-blue-500">
    <nav class="flex justify-around">
      <h1 class="sticky left-0">
        <RouterLink to="/">Url shortener</RouterLink>
      </h1>
      <RouterLink to="/about">About</RouterLink>
    </nav>
  </header>
  <main class="h-screen w-screen flex justify-center align-middle">
    <div>
      <RouterView />
    </div>
  </main>
</template>
