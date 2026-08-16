const CACHE = "bahcem-v15";
const FILES = [
  "./",
  "./index.html",
  "./bahcem.html",
  "./manifest.webmanifest",
  "./assets/mascot.png",
  "./assets/apple-touch-icon.png",
  "./assets/cloud.png",
  "./assets/garden-logo.png",
  "./assets/flowers/stage-seed.png",
  "./assets/flowers/stage-filiz.png",
  "./assets/flowers/stage-tomurcuk.png",
  "./assets/flowers/papatya.png",
  "./assets/flowers/lavanta.png",
  "./assets/flowers/cilek.png",
  "./assets/flowers/tulips.png",
  "./assets/flowers/sunflower.png",
  "./assets/avatar/girl.png",
  "./assets/avatar/boy.png",
  "./assets/flowers/icon-0.png",
  "./assets/flowers/icon-1.png",
  "./assets/flowers/icon-2.png",
  "./assets/flowers/icon-3.png",
  "./assets/flowers/icon-4.png"
];

self.addEventListener("install", (event) => {
  event.waitUntil(caches.open(CACHE).then((cache) => cache.addAll(FILES)));
  self.skipWaiting();
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches.keys().then((keys) =>
      Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k)))
    )
  );
  self.clients.claim();
});

self.addEventListener("fetch", (event) => {
  event.respondWith(
    caches.match(event.request).then((hit) => hit || fetch(event.request))
  );
});
