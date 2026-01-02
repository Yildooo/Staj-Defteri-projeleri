"use strict";

// Uygulama iki mod destekler:
// 1) Gelistirme (dogrudan OpenWeatherMap): window.OWM_API_KEY varsa client'tan istek atilir
//    (GUVENLI DEGIL - sadece lokal kullanin, uretimde kullanmayin)
// 2) Uretim (GUVENLI): /api/weather proxy (serverless) uzerinden cagirilir,
//    API anahtari sunucu tarafinda .env'de tutulur

const els = {
  form: document.getElementById("search-form"),
  input: document.getElementById("city-input"),
  btn: document.getElementById("search-btn"),
  msg: document.getElementById("message"),
  current: document.getElementById("current"),
  forecast: document.getElementById("forecast"),
  forecastGrid: document.getElementById("forecast-grid"),
  location: document.getElementById("location-name"),
  currentIcon: document.getElementById("current-icon"),
  currentTemp: document.getElementById("current-temp"),
  currentDesc: document.getElementById("current-desc"),
  feelsLike: document.getElementById("feels-like"),
  wind: document.getElementById("wind"),
  humidity: document.getElementById("humidity"),
  highLow: document.getElementById("high-low"),
  tempUnit: document.getElementById("temp-unit"),
  feelsLikeUnit: document.getElementById("feels-like-unit"),
  windUnit: document.getElementById("wind-unit"),
  hlUnit: document.getElementById("hl-unit"),
  unitC: document.getElementById("unit-c"),
  unitF: document.getElementById("unit-f"),
  locBtn: document.getElementById("loc-btn"),
  hourly: document.getElementById("hourly"),
  hourlyStrip: document.getElementById("hourly-strip"),
};

const DIRECT_KEY = typeof window !== "undefined" ? window.OWM_API_KEY : undefined;
const LANG = "tr"; // OpenWeatherMap dil kodu
const state = {
  unit: (typeof localStorage !== 'undefined' && localStorage.getItem('unit') === 'imperial') ? 'imperial' : 'metric',
  lastCity: (typeof localStorage !== 'undefined' && localStorage.getItem('lastCity')) || ''
};

function tempUnit() { return state.unit === 'imperial' ? '°F' : '°C'; }
function windUnit() { return state.unit === 'imperial' ? 'mph' : 'm/sn'; }
function owmUnits() { return state.unit === 'imperial' ? 'imperial' : 'metric'; }
function omTempUnit() { return state.unit === 'imperial' ? 'fahrenheit' : 'celsius'; }
function omWindUnit() { return state.unit === 'imperial' ? 'mph' : 'ms'; }

function setMessage(text, type = "info") {
  els.msg.textContent = text || "";
  els.msg.className = `message ${type === "error" ? "error" : type === "success" ? "success" : ""}`;
}

function setLoading(isLoading) {
  els.btn.disabled = isLoading;
  els.btn.textContent = isLoading ? "Yükleniyor..." : "Getir";
}

function iconUrl(code) {
  return `https://openweathermap.org/img/wn/${code}@2x.png`;
}

function toLocalDate(ts, tzOffsetSeconds) {
  // OpenWeather ts (saniye) + timezone_offset (saniye)
  const ms = (ts + (tzOffsetSeconds || 0)) * 1000;
  return new Date(ms);
}

function fmtDay(ts, tzOffsetSeconds) {
  const d = toLocalDate(ts, tzOffsetSeconds);
  return d.toLocaleDateString("tr-TR", { weekday: "short", day: "numeric" });
}

function fmtHour(ts, tzOffsetSeconds) {
  const d = toLocalDate(ts, tzOffsetSeconds);
  return d.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
}

// "38.25,27.30" gibi girdileri algilar
function parseLatLon(text) {
  if (!text) return null;
  const m = text.match(/^\s*([-+]?\d+(?:\.\d+)?)\s*[,;]\s*([-+]?\d+(?:\.\d+)?)\s*$/);
  if (!m) return null;
  const lat = parseFloat(m[1]);
  const lon = parseFloat(m[2]);
  if (isNaN(lat) || isNaN(lon)) return null;
  if (lat < -90 || lat > 90 || lon < -180 || lon > 180) return null;
  return { lat, lon };
}

async function fetchJSON(url) {
  const res = await fetch(url);
  if (!res.ok) {
    // OpenWeather hata durumunda 200 donmez, ok=false olur
    let reason = `${res.status} ${res.statusText}`;
    try {
      const data = await res.json();
      if (data?.message) reason += ` - ${data.message}`;
    } catch (_) {}
    throw new Error(reason);
  }
  return res.json();
}

// --- Dogrudan OpenWeatherMap (yalniz gelistirme icin) ---
async function fetchWeatherDirect(city) {
  if (!DIRECT_KEY) throw new Error("API anahtari tanimli degil.");
  const geoUrl = `https://api.openweathermap.org/geo/1.0/direct?q=${encodeURIComponent(city)}&limit=1&appid=${DIRECT_KEY}`;
  const geo = await fetchJSON(geoUrl);
  if (!Array.isArray(geo) || geo.length === 0) {
    const msg = "Sehir bulunamadi. Lutfen yazimi kontrol edin.";
    throw new Error(msg);
  }
  const { name, country, lat, lon, state } = geo[0];
  const oneCallUrl = `https://api.openweathermap.org/data/2.5/onecall?lat=${lat}&lon=${lon}&exclude=minutely,alerts&units=${owmUnits()}&lang=${LANG}&appid=${DIRECT_KEY}`;
  const oc = await fetchJSON(oneCallUrl);
  return {
    location: { city: name, country, state, lat, lon },
    current: oc.current,
    daily: oc.daily,
    hourly: (oc.hourly || []).slice(0,24),
    timezone_offset: oc.timezone_offset,
  };
}

async function fetchWeatherDirectByCoords(lat, lon) {
  if (!DIRECT_KEY) throw new Error("API anahtari tanimli degil.");
  const oneCallUrl = `https://api.openweathermap.org/data/2.5/onecall?lat=${lat}&lon=${lon}&exclude=minutely,alerts&units=${owmUnits()}&lang=${LANG}&appid=${DIRECT_KEY}`;
  const oc = await fetchJSON(oneCallUrl);
  // Ters geocoding ile isim
  const revUrl = `https://api.openweathermap.org/geo/1.0/reverse?lat=${lat}&lon=${lon}&limit=1&appid=${DIRECT_KEY}`;
  const rev = await fetchJSON(revUrl);
  const place = Array.isArray(rev) && rev[0] ? rev[0] : { name: `Bulunduğum yakın konum` };
  const displayName = (place.local_names && (place.local_names.tr || place.local_names['tr'])) || place.name;
  return {
    location: { city: displayName, country: place.country, state: place.state, lat, lon },
    current: oc.current,
    daily: oc.daily,
    hourly: (oc.hourly || []).slice(0,24),
    timezone_offset: oc.timezone_offset,
  };
}

// --- Uretim icin: Serverless proxy (/api/weather?city=...) ---
async function fetchWeatherViaProxy(city) {
  const url = `/api/weather?city=${encodeURIComponent(city)}`;
  const res = await fetch(url, { headers: { Accept: "application/json" } });
  if (!res.ok) {
    let reason = `${res.status} ${res.statusText}`;
    try { const data = await res.json(); if (data?.error) reason += ` - ${data.error}`; } catch(_){/*noop*/}
    throw new Error(reason);
  }
  return res.json();
}

// --- TAMAMEN UCRETSIZ: Open-Meteo (anahtar gerektirmez) ---
function wmoToOWMIcon(code, isDay) {
  const map = {
    0:  { d: "01d", n: "01n", t: "Acik" },
    1:  { d: "02d", n: "02n", t: "Az bulutlu" },
    2:  { d: "03d", n: "03n", t: "Parcali bulutlu" },
    3:  { d: "04d", n: "04n", t: "Cok bulutlu" },
    45: { d: "50d", n: "50n", t: "Sis" },
    48: { d: "50d", n: "50n", t: "Kiris sis" },
    51: { d: "09d", n: "09n", t: "Cisayagan" },
    53: { d: "09d", n: "09n", t: "Cisayagan" },
    55: { d: "09d", n: "09n", t: "Cisayagan" },
    56: { d: "10d", n: "10n", t: "Donan cisayagan" },
    57: { d: "10d", n: "10n", t: "Kuvvetli donan cisayagan" },
    61: { d: "10d", n: "10n", t: "Hafif yagmur" },
    63: { d: "10d", n: "10n", t: "Yagmur" },
    65: { d: "10d", n: "10n", t: "Kuvvetli yagmur" },
    66: { d: "10d", n: "10n", t: "Donan yagmur" },
    67: { d: "10d", n: "10n", t: "Kuvvetli donan yagmur" },
    71: { d: "13d", n: "13n", t: "Hafif kar" },
    73: { d: "13d", n: "13n", t: "Kar" },
    75: { d: "13d", n: "13n", t: "Kuvvetli kar" },
    77: { d: "13d", n: "13n", t: "Karcik" },
    80: { d: "09d", n: "09n", t: "Saganak" },
    81: { d: "09d", n: "09n", t: "Saganak" },
    82: { d: "09d", n: "09n", t: "Kuvvetli saganak" },
    85: { d: "13d", n: "13n", t: "Kar saganagi" },
    86: { d: "13d", n: "13n", t: "Kuvvetli kar saganagi" },
    95: { d: "11d", n: "11n", t: "Gok gurultulu saganak" },
    96: { d: "11d", n: "11n", t: "Dolu ile saganak" },
    99: { d: "11d", n: "11n", t: "Kuvvetli dolu ile saganak" },
  };
  const m = map[code] || { d: "03d", n: "03n", t: "Bulutlu" };
  return { icon: isDay ? m.d : m.n, text: m.t };
}

async function fetchWeatherOpenMeteo(city) {
  const geoUrl = `https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(city)}&count=1&language=tr`;
  const geo = await fetchJSON(geoUrl);
  if (!geo || !Array.isArray(geo.results) || geo.results.length === 0) {
    throw new Error("Sehir bulunamadi. Lutfen yazimi kontrol edin.");
  }
  const g = geo.results[0];
  const lat = g.latitude, lon = g.longitude;
  const fmUrl = `https://api.open-meteo.com/v1/forecast?latitude=${lat}&longitude=${lon}`
    + `&timezone=auto&timeformat=unixtime&windspeed_unit=${omWindUnit()}&temperature_unit=${omTempUnit()}`
    + `&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,is_day,pressure_msl`
    + `&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset&forecast_days=7`
    + `&hourly=temperature_2m,weather_code,is_day`;
  const data = await fetchJSON(fmUrl);

  const c = data.current || {};
  const mapC = wmoToOWMIcon(c.weather_code, !!c.is_day);
  const current = {
    temp: c.temperature_2m,
    feels_like: c.apparent_temperature,
    wind_speed: c.wind_speed_10m,
    humidity: c.relative_humidity_2m,
    weather: [{ icon: mapC.icon, description: mapC.text }],
  };

  const daily = [];
  const dLen = (data.daily?.time || []).length;
  for (let i = 0; i < dLen; i++) {
    const code = data.daily.weather_code[i];
    const mapD = wmoToOWMIcon(code, true);
    daily.push({
      dt: data.daily.time[i],
      temp: { max: Math.round(data.daily.temperature_2m_max[i]), min: Math.round(data.daily.temperature_2m_min[i]) },
      weather: [{ icon: mapD.icon, description: mapD.text }],
      sunrise: data.daily.sunrise?.[i],
      sunset: data.daily.sunset?.[i],
    });
  }

  const hourly = [];
  const hLen = (data.hourly?.time || []).length;
  for (let i = 0; i < Math.min(hLen, 24); i++) {
    const code = data.hourly.weather_code[i];
    const iconMap = wmoToOWMIcon(code, !!data.hourly.is_day?.[i]);
    hourly.push({
      dt: data.hourly.time[i],
      temp: data.hourly.temperature_2m[i],
      weather: [{ icon: iconMap.icon, description: iconMap.text }],
    });
  }

  return {
    location: { city: g.name, country: g.country_code || g.country, state: g.admin1, lat, lon },
    current,
    daily,
    hourly,
    timezone_offset: data.utc_offset_seconds || 0,
  };
}

async function fetchWeatherOpenMeteoByCoords(lat, lon) {
  const fmUrl = `https://api.open-meteo.com/v1/forecast?latitude=${lat}&longitude=${lon}`
    + `&timezone=auto&timeformat=unixtime&windspeed_unit=${omWindUnit()}&temperature_unit=${omTempUnit()}`
    + `&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,is_day,pressure_msl`
    + `&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset&forecast_days=7`
    + `&hourly=temperature_2m,weather_code,is_day`;
  const data = await fetchJSON(fmUrl);
  // reverse geocoding for name
  const revUrl = `https://geocoding-api.open-meteo.com/v1/reverse?latitude=${lat}&longitude=${lon}&language=tr&count=1`;
  let name = `Bulunduğum yakın konum`; let country, state;
  try {
    const rev = await fetchJSON(revUrl);
    if (rev?.results?.length) {
      const r = rev.results[0];
      name = r.name || r.admin3 || r.admin2 || r.admin1 || name;
      country = r.country_code || r.country; state = r.admin1;
    }
  } catch(_){ /* noop */ }

  const c = data.current || {};
  const mapC = wmoToOWMIcon(c.weather_code, !!c.is_day);
  const current = {
    temp: c.temperature_2m,
    feels_like: c.apparent_temperature,
    wind_speed: c.wind_speed_10m,
    humidity: c.relative_humidity_2m,
    weather: [{ icon: mapC.icon, description: mapC.text }],
  };

  const daily = [];
  const dLen = (data.daily?.time || []).length;
  for (let i = 0; i < dLen; i++) {
    const code = data.daily.weather_code[i];
    const mapD = wmoToOWMIcon(code, true);
    daily.push({
      dt: data.daily.time[i],
      temp: { max: Math.round(data.daily.temperature_2m_max[i]), min: Math.round(data.daily.temperature_2m_min[i]) },
      weather: [{ icon: mapD.icon, description: mapD.text }],
      sunrise: data.daily.sunrise?.[i],
      sunset: data.daily.sunset?.[i],
    });
  }

  const hourly = [];
  const hLen = (data.hourly?.time || []).length;
  for (let i = 0; i < Math.min(hLen, 24); i++) {
    const code = data.hourly.weather_code[i];
    const iconMap = wmoToOWMIcon(code, !!data.hourly.is_day?.[i]);
    hourly.push({
      dt: data.hourly.time[i],
      temp: data.hourly.temperature_2m[i],
      weather: [{ icon: iconMap.icon, description: iconMap.text }],
    });
  }

  return {
    location: { city: name, country, state, lat, lon },
    current,
    daily,
    hourly,
    timezone_offset: data.utc_offset_seconds || 0,
  };
}

function renderCurrent(block) {
  const { location, current, daily, timezone_offset } = block;
  const today = daily?.[0];
  // Koordinat gibi gorunen isimleri daha insan-dostu bir ada cevir
  const isCoordLike = typeof location.city === 'string' && /\d+\.?\d*\s*[,;]\s*\d+\.?\d*/.test(location.city);
  const friendlyCity = isCoordLike ? 'Bulunduğum yakın konum' : location.city;
  els.location.textContent = [friendlyCity, location.state, location.country].filter(Boolean).join(", ");
  els.currentIcon.src = iconUrl(current.weather?.[0]?.icon || "01d");
  els.currentIcon.alt = current.weather?.[0]?.description || "hava";
  els.currentTemp.textContent = Math.round(current.temp);
  els.tempUnit.textContent = tempUnit();
  els.currentDesc.textContent = current.weather?.[0]?.description || "";
  els.feelsLike.textContent = Math.round(current.feels_like ?? current.temp);
  els.feelsLikeUnit.textContent = tempUnit();
  els.wind.textContent = (current.wind_speed ?? 0).toFixed(1);
  els.windUnit.textContent = windUnit();
  els.humidity.textContent = Math.round(current.humidity ?? 0);
  if (today?.temp) {
    const hi = Math.round(today.temp.max);
    const lo = Math.round(today.temp.min);
    els.highLow.textContent = `${hi}/${lo}`;
    els.hlUnit.textContent = tempUnit();
  }
  els.current.classList.remove("hidden");
}

function renderForecast(block) {
  const { daily, timezone_offset } = block;
  els.forecastGrid.innerHTML = "";
  const days = (daily || []).slice(0, 7);
  for (const d of days) {
    const card = document.createElement("div");
    card.className = "forecast-item";
    card.innerHTML = `
      <div class="forecast-top">
        <img class="weather-icon" alt="" src="${iconUrl(d.weather?.[0]?.icon || "01d")}"/>
        <div>
          <div class="forecast-date">${fmtDay(d.dt, timezone_offset)}</div>
          <div class="forecast-desc">${d.weather?.[0]?.description || ""}</div>
        </div>
      </div>
      <div class="forecast-temps">
        <div class="temp-max">${Math.round(d.temp?.max)}°</div>
        <div class="temp-min">${Math.round(d.temp?.min)}°</div>
      </div>`;
    els.forecastGrid.appendChild(card);
  }
  els.forecast.classList.remove("hidden");
}

function renderHourly(block) {
  const { hourly, timezone_offset } = block;
  if (!hourly || hourly.length === 0) { els.hourly.classList.add('hidden'); return; }
  els.hourlyStrip.innerHTML = '';
  for (const h of hourly) {
    const div = document.createElement('div');
    div.className = 'hourly-item';
    div.innerHTML = `
      <div class="hourly-time">${fmtHour(h.dt, timezone_offset)}</div>
      <img class="hourly-icon" alt="" src="${iconUrl(h.weather?.[0]?.icon || '01d')}">
      <div class="hourly-temp">${Math.round(h.temp)}${tempUnit()}</div>
    `;
    els.hourlyStrip.appendChild(div);
  }
  els.hourly.classList.remove('hidden');
}

async function getWeather(city) {
  // Once OWM anahtari var ise OWM'i deneriz; hata alirsak veya anahtar yoksa Open‑Meteo'ya duseriz
  if (DIRECT_KEY) {
    try { return await fetchWeatherDirect(city); }
    catch (_) { /* fallback */ }
  }
  // Proxy kullanmak istiyorsaniz: return fetchWeatherViaProxy(city);
  return fetchWeatherOpenMeteo(city);
}

async function getWeatherByCoords(lat, lon) {
  if (DIRECT_KEY) {
    try { return await fetchWeatherDirectByCoords(lat, lon); }
    catch(_) { /* fallback */ }
  }
  return fetchWeatherOpenMeteoByCoords(lat, lon);
}

async function onSubmit(e) {
  e.preventDefault();
  const city = (els.input.value || "").trim();
  if (!city) {
    setMessage("Lutfen bir sehir girin.", "error");
    return;
  }
  setMessage("");
  setLoading(true);
  try {
    const coords = parseLatLon(city);
    const data = coords ? await getWeatherByCoords(coords.lat, coords.lon) : await getWeather(city);
    renderCurrent(data);
    renderForecast(data);
    renderHourly(data);
    setMessage("Veriler guncellendi.", "success");
    try {
      if (typeof localStorage !== 'undefined') localStorage.setItem('lastCity', data.location?.city || city);
      if (data?.location?.city) els.input.value = data.location.city;
    } catch(_){}
  } catch (err) {
    console.error(err);
    const isNoKey = !DIRECT_KEY;
    const hint = isNoKey ?
      " (Not: Uretimde /api/weather proxy kullanmali, gelistirme icin config.local.js ile window.OWM_API_KEY tanimlayabilirsiniz.)" : "";
    setMessage(`Bir hata olustu: ${err.message}${hint}`, "error");
  } finally {
    setLoading(false);
  }
}

els.form.addEventListener("submit", onSubmit);
els.input.addEventListener("keydown", (e) => {
  if (e.key === "Enter") els.form.requestSubmit();
});

function updateUnitUI() {
  els.unitC.classList.toggle('active', state.unit === 'metric');
  els.unitF.classList.toggle('active', state.unit === 'imperial');
}

async function refetchWithCurrentInput() {
  const city = (els.input.value || state.lastCity || '').trim();
  if (city) {
    els.form.dispatchEvent(new Event('submit', { cancelable: true }));
  }
}

els.unitC?.addEventListener('click', async () => {
  state.unit = 'metric';
  try { localStorage.setItem('unit', 'metric'); } catch(_){ }
  updateUnitUI();
  await refetchWithCurrentInput();
});
els.unitF?.addEventListener('click', async () => {
  state.unit = 'imperial';
  try { localStorage.setItem('unit', 'imperial'); } catch(_){ }
  updateUnitUI();
  await refetchWithCurrentInput();
});

els.locBtn?.addEventListener('click', () => {
  if (!navigator.geolocation) {
    setMessage('Tarayiciniz konum servisini desteklemiyor.', 'error');
    return;
  }
  setMessage('Konum aliniyor...');
  navigator.geolocation.getCurrentPosition(async (pos) => {
    try {
      setLoading(true);
      const { latitude, longitude } = pos.coords;
      const data = await getWeatherByCoords(latitude, longitude);
      renderCurrent(data);
      renderForecast(data);
      renderHourly(data);
      setMessage('Konuma gore veriler guncellendi.', 'success');
      try {
        if (data?.location?.city) {
          els.input.value = data.location.city;
          if (typeof localStorage !== 'undefined') localStorage.setItem('lastCity', data.location.city);
        }
      } catch(_){ }
    } catch(err){
      console.error(err);
      setMessage(`Konumdan veri alinamadi: ${err.message}`, 'error');
    } finally {
      setLoading(false);
    }
  }, (err) => {
    setMessage('Konum izni gerek veya reddedildi.', 'error');
  }, { enableHighAccuracy: false, timeout: 10000, maximumAge: 60000 });
});

// Initial UI state
updateUnitUI();
try {
  if (state.lastCity) {
    els.input.value = state.lastCity;
  }
} catch(_){ }
