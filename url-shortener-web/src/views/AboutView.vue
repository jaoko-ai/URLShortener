<template>
  <article class="mx-auto max-w-2xl">
    <p class="text-xs uppercase tracking-[0.22em] text-ember">About</p>
    <h1 class="mt-2 font-display text-5xl leading-tight">A frontend waiting on a real API.</h1>
    <div class="mt-6 space-y-4 text-lg leading-relaxed text-muted">
      <p>
        Shortpath is a Vue 3 template for a URL shortener. It already talks to the
        endpoints documented in
        <code class="text-ink">url-shortener-api</code>
        so you can practise ASP.NET without building a UI first.
      </p>
      <p>
        Guests can shorten links. Accounts, JWT sessions, and per-user history are
        ready in Pinia as soon as
        <code class="text-ink">/api/auth/*</code>
        exists.
      </p>
    </div>
  </article>
</template>
