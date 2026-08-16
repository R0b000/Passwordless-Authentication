namespace Test
{
    public static class HtmlCode
    {
        public const string Source = """
           <!DOCTYPE html>
<html lang = "en" >
< head >
< meta charset="UTF-8" />
<meta name = "viewport" content="width=device-width, initial-scale=1.0" />
<title>NOVAPULSE® — Digital Growth Agency</title>
<meta name = "description" content="Novapulse is a full-funnel digital marketing agency. We turn attention into revenue with performance marketing, SEO, social and creative." />

<link rel = "stylesheet" href="https://cdn.jsdelivr.net/npm/@fontsource-variable/syne@5/index.css" />
<link rel = "stylesheet" href="https://cdn.jsdelivr.net/npm/@fontsource-variable/instrument-sans@5/index.css" />
<link rel = "stylesheet" href="https://cdn.jsdelivr.net/npm/@fontsource/space-mono@5.0.8/400.css" />
<link rel = "stylesheet" href="https://cdn.jsdelivr.net/npm/@fontsource/space-mono@5.0.8/700.css" />

<script src = "https://cdn.tailwindcss.com" ></ script >
< script >
tailwind.config = {
  theme: {
    extend: {
      colors: {
        ink:  '#0A0A0C',
        coal: '#131317',
        slate2:'#1B1B21',
        bone: '#F3F0E7',
        acid: '#C9F73A',
        grape:'#8C7CFF',
        mango:'#FF6B3D',
      },
      fontFamily:
{
display: ['"Syne Variable"', 'sans-serif'],
        body: ['"Instrument Sans Variable"', 'sans-serif'],
        mono: ['"Space Mono"', 'monospace'],
      },
    }
  }
}
</ script >

< style >
  html {
background:#0A0A0C; }
  body { overflow - x:hidden; }
  ::selection {
    background:#C9F73A; color:#0A0A0C; }
  ::- webkit - scrollbar { width: 10px; }
  ::- webkit - scrollbar - track {
        background:#0A0A0C; }
  ::- webkit - scrollbar - thumb {
            background:#2a2a31; border-radius:8px; }
  ::- webkit - scrollbar - thumb:hover {
                background:#C9F73A; }

  /* ---------- noise overlay ---------- */
  .np - noise{
                    position: fixed; inset: -50 %; width: 200 %; height: 200 %;
                        background - image:url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="240" height="240"><filter id="n"><feTurbulence type="fractalNoise" baseFrequency="0.85" numOctaves="2" stitchTiles="stitch"/></filter><rect width="240" height="240" filter="url(%23n)" opacity="0.55"/></svg>');
                    opacity: .055; pointer - events:none; z - index:70;
                    animation: npGrain 9s steps(10) infinite;
                    }
                    @keyframes npGrain{
                        0 %,100 %{ transform: translate(0, 0)}
                        10 %{ transform: translate(-3 %, 2 %)}
                        20 %{ transform: translate(2 %, -4 %)}
                        30 %{ transform: translate(-4 %, -2 %)}
                        40 %{ transform: translate(3 %, 3 %)}
                        50 %{ transform: translate(-2 %, 4 %)}
                        60 %{ transform: translate(4 %, -3 %)}
                        70 %{ transform: translate(-3 %, -3 %)}
                        80 %{ transform: translate(2 %, 2 %)}
                        90 %{ transform: translate(-4 %, 3 %)}
                    }

                    /* ---------- custom cursor ---------- */
# npDot,#npRing{ position:fixed; top:0; left:0; pointer-events:none; z-index:90; border-radius:50%; }
# npDot{ width:8px; height:8px; background:#C9F73A; transform:translate(-50%,-50%); }
# npRing{ width:38px; height:38px; border:1.5px solid rgba(201,247,58,.65); transform:translate(-50%,-50%); transition:width .3s,height .3s,border-color .3s,background .3s; }
                    body.np - cur - hover #npRing{ width:64px; height:64px; background:rgba(201,247,58,.12); border-color:#C9F73A; }
  @media(hover: none), (pointer: coarse){ #npDot,#npRing{ display:none; } }
  @media(hover: hover) and(pointer: fine){ body { cursor: none; } a,button{ cursor: none; } }

  /* ---------- loader ---------- */
  #npLoader{ position:fixed; inset:0; z-index:100; background:#0A0A0C; display:flex; align-items:center; justify-content:center; }
  #npLoader .np-lword span{ display:inline-block; overflow:hidden; }
  #npLoader .np-lword span i{ display:inline-block; font-style:normal; transform:translateY(110%); }

  /* ---------- marquee ---------- */
  .np - marquee{ overflow: hidden; }
  .np - mtrack{ display: flex; width: max - content; animation: npMarquee var(--mdur,28s) linear infinite; }
  .np - marquee:hover.np - mtrack{ animation - play - state:paused; }
                        @keyframes npMarquee{ to{ transform: translateX(-50 %); } }

  /* ---------- line-mask reveal ---------- */
  .np - rl{ display: block; overflow: hidden; }
  .np - rl > span{ display: block; transform: translateY(115 %); will - change:transform; }

  /* ---------- outline text ---------- */
  .np - outline{ color: transparent; -webkit - text - stroke:1.5px rgba(243,240,231,.85); }
  .np - outline - acid{
                        color: transparent; -webkit - text - stroke:1.5px #C9F73A; }

  /* ---------- misc motion ---------- */
  @keyframes npSpin{ to{ transform: rotate(360deg); } }
  .np - spin{ animation: npSpin 14s linear infinite; }
  .np - spin - slow{ animation: npSpin 26s linear infinite; }
                            @keyframes npFloat{ 0 %,100 %{ transform: translateY(0); } 50 %{ transform: translateY(-14px); } }
  .np - float{ animation: npFloat 5s ease-in-out infinite; }
                            @keyframes npPulse{ 0 %{ box - shadow:0 0 0 0 rgba(201, 247, 58, .6); } 100 %{ box - shadow:0 0 0 14px rgba(201,247,58,0); } }
  .np - pulse{ animation: npPulse 1.6s ease-out infinite; }
                            @keyframes npKen{ from{ transform: scale(1) translate(0, 0); } to{ transform: scale(1.12) translate(2 %, -2 %); } }
  .np - kb img{ animation: npKen 14s ease-in-out infinite alternate; }
                            @keyframes npBar{ from{ transform: scaleY(.25); } to{ transform: scaleY(1); } }
  .np - eq span{ transform - origin:bottom; animation: npBar 1s ease-in-out infinite alternate; }

  /* hero grid bg */
  .np - gridbg{
                                background - image:linear - gradient(rgba(243, 240, 231, .05) 1px, transparent 1px),linear - gradient(90deg, rgba(243, 240, 231, .05) 1px, transparent 1px);
                                background - size:72px 72px;
                                mask - image:radial - gradient(ellipse 90 % 80 % at 50 % 40 %, black 30 %, transparent 75 %);
                            }

  /* underline sweep for nav links */
  .np - link{ position: relative; }
  .np - link::after{
                            content: ''; position: absolute; left: 0; bottom: -4px; height: 2px; width: 100 %; background:#C9F73A; transform:scaleX(0); transform-origin:right; transition:transform .35s cubic-bezier(.76,0,.24,1); }
  .np - link:hover::after{ transform: scaleX(1); transform - origin:left; }

  /* buttons */
  .np - btn - acid{ position: relative; overflow: hidden; isolation: isolate; }
  .np - btn - acid::before{
                                content: ''; position: absolute; inset: 0; background:#F3F0E7; transform:translateY(101%); transition:transform .4s cubic-bezier(.76,0,.24,1); z-index:-1; }
  .np - btn - acid:hover::before{ transform: translateY(0); }

  /* service cards */
  .np - svc{ will - change:transform; }

  /* tilt cards */
  .np - tilt{ transform - style:preserve - 3d; will - change:transform; }
  .np - tilt.np - tilt - pop{ transform: translateZ(30px); }

  /* faq */
  .np - faq - a{ max - height:0; overflow: hidden; transition: max - height .5s cubic-bezier(.76, 0, .24, 1); }
  .np - faq - item.np - faq - x{ transition: transform .4s; }
  .np - faq - item.open.np - faq - x{ transform: rotate(45deg); }
  .np - faq - item.open{ border - color:rgba(201, 247, 58, .5); }

  /* testimonials */
  .np - tslide{ position: absolute; inset: 0; opacity: 0; transform: translateY(30px); transition: opacity .7s,transform .7s; pointer - events:none; }
  .np - tslide.active{ opacity: 1; transform: translateY(0); pointer - events:auto; }
  #npTprog{ transform-origin:left; transform:scaleX(0); }

  /* progress bar */
  #npProgress{ transform-origin:left; transform:scaleX(0); }

  /* mobile menu */
  #npMenu{ clip-path:inset(0 0 100% 0); transition:clip-path .7s cubic-bezier(.76,0,.24,1); }
  #npMenu.open{ clip-path:inset(0 0 0% 0); }
  #npMenu .np-mlink{ opacity:0; transform:translateY(40px); transition:opacity .5s,transform .5s; }
  #npMenu.open .np-mlink{ opacity:1; transform:translateY(0); }

  /* header */
  #npHeader{ transition:background .4s,border-color .4s,backdrop-filter .4s; border-bottom:1px solid transparent; }
  #npHeader.scrolled{ background:rgba(10,10,12,.8); backdrop-filter:blur(14px); border-bottom-color:rgba(243,240,231,.08); }

  /* impact bars */
  .np - impbar{ transform: scaleY(0); transform - origin:bottom; }

                                    /* reduced motion */
                                    @media(prefers - reduced - motion: reduce){
    .np - mtrack,.np - spin,.np - spin - slow,.np - float,.np - pulse,.np - kb img,.np - eq span,.np - noise{ animation: none!important; }
                                        html{ scroll - behavior:auto; }
                                        *{ transition - duration:.01ms!important; }
                                    }
</ style >
</ head >

< body class= "bg-ink text-bone font-body antialiased" >

< !-- ============ LOADER ============ -->
< div id = "npLoader" >
  < div class= "text-center" >
    < div class= "np-lword font-display font-extrabold text-4xl md:text-6xl tracking-tight text-bone" aria - hidden = "true" >
      < span >< i > N </ i ></ span >< span >< i > O </ i ></ span >< span >< i > V </ i ></ span >< span >< i > A </ i ></ span >< span class= "text-acid" >< i > P </ i ></ span >< span class= "text-acid" >< i > U </ i ></ span >< span class= "text-acid" >< i > L </ i ></ span >< span class= "text-acid" >< i > S </ i ></ span >< span class= "text-acid" >< i > E </ i ></ span >
    </ div >
    < div class= "mt-6 font-mono text-sm text-bone/60" >< span id = "npLoadPct" > 000 </ span >% — CALIBRATING GROWTH ENGINE</div>
    <div class= "mt-3 h-px w-56 mx-auto bg-bone/15 overflow-hidden" >< div id = "npLoadBar" class= "h-full bg-acid" style = "width:0%" ></ div ></ div >
  </ div >
</ div >

< div class= "np-noise" aria - hidden = "true" ></ div >
< div id = "npDot" aria - hidden = "true" ></ div >< div id = "npRing" aria - hidden = "true" ></ div >
< div class= "fixed top-0 left-0 right-0 h-[3px] z-[75] bg-bone/10" >< div id = "npProgress" class= "h-full bg-acid" ></ div ></ div >

< !-- ============ HEADER ============ -->
< header id = "npHeader" class= "fixed top-0 left-0 right-0 z-50" >
  < div class= "max-w-[1400px] mx-auto px-5 md:px-10 h-[76px] flex items-center justify-between" >
    < a href = "#top" class= "np-navlink flex items-center gap-2 group" data - cursor >
      < svg width = "30" height = "30" viewBox = "0 0 32 32" fill = "none" class= "group-hover:rotate-90 transition-transform duration-500" >
        < rect x = "1" y = "1" width = "30" height = "30" rx = "8" stroke = "#C9F73A" stroke - width = "2" />
        < path d = "M8 20 L13 20 L15 12 L18 22 L20 15 L24 15" stroke = "#C9F73A" stroke - width = "2.4" stroke - linecap = "round" stroke - linejoin = "round" />
      </ svg >
      < span class= "font-display font-extrabold text-lg tracking-tight" > NOVA < span class= "text-acid" > PULSE </ span >< span class= "text-[10px] align-top" >®</ span ></ span >
    </ a >

    < nav class= "hidden lg:flex items-center gap-8 font-mono text-[13px] uppercase tracking-widest text-bone/70" >
      < a class= "np-link hover:text-bone transition-colors" href = "#services" data - cursor > Services </ a >
      < a class= "np-link hover:text-bone transition-colors" href = "#process" data - cursor > Process </ a >
      < a class= "np-link hover:text-bone transition-colors" href = "#work" data - cursor > Work </ a >
      < a class= "np-link hover:text-bone transition-colors" href = "#pricing" data - cursor > Pricing </ a >
      < a class= "np-link hover:text-bone transition-colors" href = "#faq" data - cursor > FAQ </ a >
    </ nav >

    < div class= "flex items-center gap-4" >
      < a href = "#contact" data - cursor data - magnetic class= "np-btn-acid hidden md:inline-flex items-center gap-2 bg-acid text-ink font-mono text-[12px] uppercase tracking-widest font-bold px-5 py-3 rounded-full" >
        Book a call<span aria-hidden="true">→</span>
      </a>
      <button id = "npBurger" data-cursor aria-label= "Open menu" class= "lg:hidden w-11 h-11 rounded-full border border-bone/20 flex flex-col items-center justify-center gap-1.5" >
        < span class= "np-b1 block w-5 h-[2px] bg-bone transition-transform" ></ span >
        < span class= "np-b2 block w-5 h-[2px] bg-bone transition-transform" ></ span >
      </ button >
    </ div >
  </ div >
</ header >

< !--mobile menu-- >
< div id = "npMenu" class= "fixed inset-0 z-[80] bg-ink flex flex-col justify-center px-8" >
  < button id = "npClose" data - cursor aria - label = "Close menu" class= "absolute top-6 right-6 w-12 h-12 rounded-full border border-bone/25 font-mono text-lg text-acid" >✕</ button >
  < nav class= "flex flex-col gap-2 font-display font-extrabold text-5xl md:text-6xl uppercase tracking-tight" >
    < a class= "np-mlink hover:text-acid transition-colors" style = "transition-delay:.15s" href = "#services" > Services </ a >
    < a class= "np-mlink hover:text-acid transition-colors" style = "transition-delay:.22s" href = "#process" > Process </ a >
    < a class= "np-mlink hover:text-acid transition-colors" style = "transition-delay:.29s" href = "#work" > Work </ a >
    < a class= "np-mlink hover:text-acid transition-colors" style = "transition-delay:.36s" href = "#pricing" > Pricing </ a >
    < a class= "np-mlink hover:text-acid transition-colors" style = "transition-delay:.43s" href = "#contact" > Contact </ a >
  </ nav >
  < p class= "np-mlink mt-10 font-mono text-xs text-bone/50 tracking-widest" style = "transition-delay:.5s" > HELLO@NOVAPULSE.AGENCY — < span class= "np-clock" > 00:00:00 </ span ></ p >
</ div >

< main id = "top" >

< !-- ============ HERO ============ -->
< section class= "relative min-h-screen flex flex-col justify-between overflow-hidden pt-[76px]" >
  < div class= "absolute inset-0 np-gridbg" aria - hidden = "true" ></ div >
  < div class= "absolute -top-40 right-[-10%] w-[720px] h-[720px] rounded-full opacity-25" style = "background:radial-gradient(circle,#C9F73A 0%,transparent 60%)" aria - hidden = "true" ></ div >
  < svg class= "absolute top-1/2 right-[-260px] -translate-y-1/2 w-[640px] h-[640px] np-spin-slow opacity-40 hidden md:block" viewBox = "0 0 200 200" aria - hidden = "true" >
    < circle cx = "100" cy = "100" r = "88" fill = "none" stroke = "#C9F73A" stroke - width = "1" stroke - dasharray = "4 10" />
    < circle cx = "100" cy = "100" r = "62" fill = "none" stroke = "#8C7CFF" stroke - width = "1" stroke - dasharray = "2 8" />
    < circle cx = "100" cy = "100" r = "34" fill = "none" stroke = "#F3F0E7" stroke - width = ".6" stroke - dasharray = "1 6" />
  </ svg >

  < div class= "relative max-w-[1400px] mx-auto px-5 md:px-10 w-full pt-10 md:pt-16 pb-8 grid lg:grid-cols-12 gap-10 items-end flex-1" >
    < !--left-- >
    < div class= "lg:col-span-8" >
      < div id = "npHeroMeta" class= "flex flex-wrap items-center gap-4 font-mono text-[11px] md:text-xs uppercase tracking-widest text-bone/60" >
        < span class= "inline-flex items-center gap-2 border border-acid/40 text-acid rounded-full px-3 py-1.5" >< span class= "w-2 h-2 rounded-full bg-acid np-pulse" ></ span > Accepting Q3 clients</span>
        <span>NYC / LDN / Remote</span>
        <span class= "hidden sm:inline" > LOCAL TIME — <span class= "np-clock text-bone" > 00:00:00 </ span ></ span >
      </ div >

      < h1 class= "mt-8 font-display font-extrabold uppercase leading-[0.92] tracking-tight text-[13.5vw] sm:text-[11vw] lg:text-[7.6vw]" >
        < span class= "np-rl" >< span > We turn </ span ></ span >
        < span class= "np-rl" >< span class= "text-acid" >< span id = "npRotWord" class= "inline-block min-w-[6.5ch]" > scrolls </ span ></ span ></ span >
        < span class= "np-rl" >< span > into < em class= "np-outline not-italic" > revenue.</ em ></ span ></ span >
      </ h1 >

      < div class= "mt-8 flex flex-col md:flex-row md:items-center gap-6" >
        < p id = "npHeroSub" class= "max-w-md text-bone/65 text-base md:text-lg leading-relaxed" >
          Novapulse is the full - funnel growth studio behind the internet's loudest brands. Strategy, creative and media — engineered as one machine.
        </p>
        <div id="npHeroCtas" class= "flex items-center gap-4" >
          < a href = "#contact" data - cursor data - magnetic class= "np-btn-acid inline-flex items-center gap-2 bg-acid text-ink font-mono text-[12px] uppercase tracking-widest font-bold px-7 py-4 rounded-full" > Start a project →</a>
          <a href="#work" data-cursor class= "inline-flex items-center gap-2 border border-bone/25 hover:border-acid hover:text-acid transition-colors font-mono text-[12px] uppercase tracking-widest px-7 py-4 rounded-full" > See the work ↓</a>
        </div>
      </div>
    </div>

    <!-- right : live card -->
    <div class= "lg:col-span-4 relative" >
      < div id = "npHeroCard" class= "np-float relative bg-coal/90 border border-bone/10 rounded-2xl p-6 shadow-[0_30px_80px_rgba(0,0,0,.5)]" >
        < div class= "flex items-center justify-between font-mono text-[11px] uppercase tracking-widest text-bone/50" >
          < span > Live campaign </ span >< span class= "text-acid" >● REC </ span >
        </ div >
        < div class= "mt-4 flex items-end justify-between" >
          < div >
            < div class= "font-mono text-[11px] text-bone/50 uppercase tracking-widest" > Blended ROAS </ div >
            < div class= "font-display font-extrabold text-5xl text-acid" >< span data - count = "4.8" data - decimals = "1" > 0 </ span > x </ div >
          </ div >
          < div class= "np-eq flex items-end gap-1 h-16" aria - hidden = "true" >
            < span class= "w-2 bg-acid/80 rounded-sm" style = "height:30%; animation-delay:0s" ></ span >
            < span class= "w-2 bg-acid rounded-sm" style = "height:55%; animation-delay:.12s" ></ span >
            < span class= "w-2 bg-grape rounded-sm" style = "height:42%; animation-delay:.24s" ></ span >
            < span class= "w-2 bg-acid rounded-sm" style = "height:70%; animation-delay:.36s" ></ span >
            < span class= "w-2 bg-acid/80 rounded-sm" style = "height:58%; animation-delay:.48s" ></ span >
            < span class= "w-2 bg-mango rounded-sm" style = "height:82%; animation-delay:.6s" ></ span >
            < span class= "w-2 bg-acid rounded-sm" style = "height:100%; animation-delay:.72s" ></ span >
          </ div >
        </ div >
        < div class= "mt-5 grid grid-cols-3 gap-3 border-t border-bone/10 pt-4 font-mono text-[11px]" >
          < div >< div class= "text-bone/50" > CTR </ div >< div class= "text-bone text-sm mt-1" > 9.4 %</ div ></ div >
          < div >< div class= "text-bone/50" > CAC </ div >< div class= "text-bone text-sm mt-1" >−38 %</ div ></ div >
          < div >< div class= "text-bone/50" > LTV </ div >< div class= "text-acid text-sm mt-1" > +212 %</ div ></ div >
        </ div >
      </ div >
      < !--rotating badge-- >
      < div class= "absolute -top-14 -left-14 w-28 h-28 hidden md:block" aria - hidden = "true" >
        < svg viewBox = "0 0 100 100" class= "w-full h-full np-spin" >
          < defs >< path id = "npCirc" d = "M50,50 m-38,0 a38,38 0 1,1 76,0 a38,38 0 1,1 -76,0" /></ defs >
          < text fill = "#C9F73A" font - size = "10.5" letter - spacing = "2.5" font - family = "Space Mono, monospace" >
            < textPath href = "#npCirc" > GROWTH • CREATIVE • MEDIA • DATA •</textPath>
          </text>
        </svg>
        <div class= "absolute inset-0 flex items-center justify-center text-acid text-xl" >↗</ div >
      </ div >
    </ div >
  </ div >

  < !--hero stats strip -->
  <div class= "relative border-t border-bone/10" >
    < div class= "max-w-[1400px] mx-auto px-5 md:px-10 grid grid-cols-2 md:grid-cols-4 divide-x divide-bone/10" >
      < div class= "py-6 px-4" >< div class= "font-display font-extrabold text-3xl md:text-4xl text-bone" >< span data - count = "240" > 0 </ span > +</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Brands scaled </ div ></ div >
      < div class= "py-6 px-4" >< div class= "font-display font-extrabold text-3xl md:text-4xl text-bone" >$< span data - count = "86" > 0 </ span > M </ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Revenue generated </ div ></ div >
      < div class= "py-6 px-4" >< div class= "font-display font-extrabold text-3xl md:text-4xl text-acid" >< span data - count = "98" > 0 </ span >%</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Client retention </ div ></ div >
      < div class= "py-6 px-4" >< div class= "font-display font-extrabold text-3xl md:text-4xl text-bone" >< span data - count = "12" > 0 </ span ></ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Years in the game</div></div>
    </div>
  </div>

  <!-- keyword marquee -->
  <div class= "np-marquee bg-acid text-ink border-y border-ink/20 py-4" aria - hidden = "true" >
    < div class= "np-mtrack font-display font-extrabold uppercase tracking-tight text-xl md:text-2xl" style = "--mdur:24s" >
      < div class= "flex shrink-0 items-center" >
        < span class= "px-6" > Performance Marketing </ span >< span >✦</ span >< span class= "px-6" > SEO & Content </ span >< span >✦</ span >< span class= "px-6" > Social & Creator </ span >< span >✦</ span >< span class= "px-6" > Brand & Creative </ span >< span >✦</ span >< span class= "px-6" > Analytics & CRO </ span >< span >✦</ span >< span class= "px-6" > Email & Lifecycle </ span >< span >✦</ span >
      </ div >
      < div class= "flex shrink-0 items-center" >
        < span class= "px-6" > Performance Marketing </ span >< span >✦</ span >< span class= "px-6" > SEO & Content </ span >< span >✦</ span >< span class= "px-6" > Social & Creator </ span >< span >✦</ span >< span class= "px-6" > Brand & Creative </ span >< span >✦</ span >< span class= "px-6" > Analytics & CRO </ span >< span >✦</ span >< span class= "px-6" > Email & Lifecycle </ span >< span >✦</ span >
      </ div >
    </ div >
  </ div >
</ section >

< !-- ============ CLIENT MARQUEE ============ -->
< section class= "py-14 border-b border-bone/10 overflow-hidden" >
  < p class= "text-center font-mono text-[11px] uppercase tracking-[0.3em] text-bone/40 mb-8" data - scramble > Trusted by category killers worldwide</p>
  <div class= "np-marquee" aria - hidden = "true" >
    < div class= "np-mtrack items-center opacity-70" style = "--mdur:36s" >
      < div class= "flex shrink-0 items-center gap-16 pr-16 font-display font-bold text-2xl md:text-3xl text-bone/60" >
        < span > KARTLY </ span >< span class= "text-acid" >◆</ span >< span class= "italic" > finlo </ span >< span class= "text-acid" >◆</ span >< span > BREWTREE </ span >< span class= "text-acid" >◆</ span >< span class= "tracking-[0.3em]" > AERIS </ span >< span class= "text-acid" >◆</ span >< span class= "italic" > Volt & amp; Co </ span >< span class= "text-acid" >◆</ span >< span > MODU </ span >< span class= "text-acid" >◆</ span >< span class= "tracking-[0.2em]" > LUMEN </ span >< span class= "text-acid" >◆</ span >< span class= "italic" > novex_ </ span >< span class= "text-acid" >◆</ span >
      </ div >
      < div class= "flex shrink-0 items-center gap-16 pr-16 font-display font-bold text-2xl md:text-3xl text-bone/60" >
        < span > KARTLY </ span >< span class= "text-acid" >◆</ span >< span class= "italic" > finlo </ span >< span class= "text-acid" >◆</ span >< span > BREWTREE </ span >< span class= "text-acid" >◆</ span >< span class= "tracking-[0.3em]" > AERIS </ span >< span class= "text-acid" >◆</ span >< span class= "italic" > Volt & amp; Co </ span >< span class= "text-acid" >◆</ span >< span > MODU </ span >< span class= "text-acid" >◆</ span >< span class= "tracking-[0.2em]" > LUMEN </ span >< span class= "text-acid" >◆</ span >< span class= "italic" > novex_ </ span >< span class= "text-acid" >◆</ span >
      </ div >
    </ div >
  </ div >
</ section >

< !-- ============ MANIFESTO ============ -->
< section class= "py-24 md:py-36 relative" >
  < div class= "max-w-[1400px] mx-auto px-5 md:px-10 grid lg:grid-cols-12 gap-12" >
    < div class= "lg:col-span-4" >
      < div class= "lg:sticky lg:top-28" >
        < p class= "font-mono text-xs uppercase tracking-[0.3em] text-acid" data - scramble > (01) — The Studio</p>
        <div class= "mt-8 rounded-2xl overflow-hidden border border-bone/10 np-kb" >
          < img src = "https://image.qwenlm.ai/public_source/7b1b5bdb-9e22-4a48-82f3-e6d260853da5/1b51509e4-51c1-4091-b9fc-2cd655be4429.png" alt = "Inside the Novapulse growth studio" class= "w-full h-72 object-cover" loading = "lazy" />
        </ div >
        < p class= "mt-4 font-mono text-[11px] text-bone/40 uppercase tracking-widest" > HQ — Data Driven Creativity, always on.</p>
      </div>
    </div>
    <div class= "lg:col-span-8" >
      < h2 class= "font-display font-extrabold uppercase tracking-tight leading-[1.02] text-3xl sm:text-5xl lg:text-6xl" >
        < span class= "np-rl" >< span > Most agencies sell noise.</span></span>
        <span class= "np-rl" >< span class= "text-acid" > We engineer growth.</ span ></ span >
      </ h2 >
      < div class= "mt-10 grid md:grid-cols-2 gap-10 text-bone/65 leading-relaxed" >
        < p class= "np-fade" > Every dollar you spend is wired to a metric that matters. We fuse scroll-stopping creative with ruthless media buying and a testing engine that never sleeps — so your brand compounds while competitors guess.</p>
        <p class= "np-fade" > One squad, one roadmap, one number on the wall: < span class= "text-bone font-semibold" > your revenue.</ span > No 40 - slide decks, no vanity dashboards. Just shipped experiments, weekly learnings and graphs that go up and to the right.</p>
      </div>
      <div class= "mt-12 grid grid-cols-2 md:grid-cols-4 gap-6" >
        < div class= "border-l-2 border-acid pl-4 np-fade" >< div class= "font-display font-extrabold text-4xl text-bone" >< span data - count = "34" > 0 </ span ></ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Industry awards </ div ></ div >
        < div class= "border-l-2 border-grape pl-4 np-fade" >< div class= "font-display font-extrabold text-4xl text-bone" >< span data - count = "52" > 0 </ span ></ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Strategists & makers </ div ></ div >
        < div class= "border-l-2 border-mango pl-4 np-fade" >< div class= "font-display font-extrabold text-4xl text-bone" >< span data - count = "1400" > 0 </ span > +</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Experiments / yr </ div ></ div >
        < div class= "border-l-2 border-bone pl-4 np-fade" >< div class= "font-display font-extrabold text-4xl text-bone" >< span data - count = "4.9" data - decimals = "1" > 0 </ span ></ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Avg.client rating </ div ></ div >
      </ div >
    </ div >
  </ div >
</ section >

< !-- ============ SERVICES : STACKED CARDS ============ -->
<section id="services" class= "py-24 md:py-32 bg-coal border-y border-bone/10" >
  < div class= "max-w-[1400px] mx-auto px-5 md:px-10" >
    < div class= "flex flex-wrap items-end justify-between gap-6 mb-16" >
      < div >
        < p class= "font-mono text-xs uppercase tracking-[0.3em] text-acid" data - scramble > (02) — Capabilities </ p >
        < h2 class= "mt-4 font-display font-extrabold uppercase tracking-tight text-4xl md:text-6xl leading-none" >
          < span class= "np-rl" >< span > Full - funnel.</ span ></ span >
          < span class= "np-rl" >< span class= "np-outline" > Full throttle.</ span ></ span >
        </ h2 >
      </ div >
      < p class= "max-w-sm text-bone/55 np-fade" > Four disciplines, one operating system. Scroll to stack the deck — every service plugs into the same growth loop.</p>
    </div>

    <div id="npStack" class= "relative" >
      < !--card 1-- >
      < article class= "np-svc sticky top-[90px] mb-8 rounded-3xl bg-slate2 border border-bone/10 p-8 md:p-14 grid md:grid-cols-12 gap-8 items-center min-h-[62vh]" >
        < div class= "md:col-span-1 font-mono text-acid text-lg" >/ 01 </ div >
        < div class= "md:col-span-6" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > Performance Marketing </ h3 >
          < p class= "mt-5 text-bone/60 max-w-lg leading-relaxed" > Paid social & search engineered like a trading desk. Creative rotation, bid strategy and landing CRO in one loop, tuned daily by humans who love numbers.</p>
          <div class= "mt-6 flex flex-wrap gap-2 font-mono text-[11px] uppercase tracking-widest" >
            < span class= "border border-acid/40 text-acid rounded-full px-3 py-1" > Meta </ span >< span class= "border border-acid/40 text-acid rounded-full px-3 py-1" > Google </ span >< span class= "border border-acid/40 text-acid rounded-full px-3 py-1" > TikTok </ span >< span class= "border border-acid/40 text-acid rounded-full px-3 py-1" > Programmatic </ span >< span class= "border border-acid/40 text-acid rounded-full px-3 py-1" > Retargeting </ span >
          </ div >
        </ div >
        < div class= "md:col-span-5 flex md:justify-end" >
          < div class= "bg-ink/60 border border-acid/25 rounded-2xl p-6 w-full max-w-xs" >
            < div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50" > Avg.blended ROAS </ div >
            < div class= "font-display font-extrabold text-6xl text-acid mt-2" > 4.8x </ div >
            < svg viewBox = "0 0 200 70" class= "mt-4 w-full" >< path d = "M0 60 C30 55 40 40 60 42 C85 45 95 25 120 26 C150 28 165 12 200 6" fill = "none" stroke = "#C9F73A" stroke - width = "2.5" />< circle cx = "200" cy = "6" r = "4" fill = "#C9F73A" /></ svg >
          </ div >
        </ div >
      </ article >
      < !--card 2-- >
      < article class= "np-svc sticky top-[114px] mb-8 rounded-3xl bg-[#171720] border border-bone/10 p-8 md:p-14 grid md:grid-cols-12 gap-8 items-center min-h-[62vh]" >
        < div class= "md:col-span-1 font-mono text-grape text-lg" >/ 02 </ div >
        < div class= "md:col-span-6" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > SEO & Content </ h3 >
          < p class= "mt-5 text-bone/60 max-w-lg leading-relaxed" > Own the search results page like real estate. Technical SEO, programmatic pages and editorial that ranks, converts and keeps compounding long after the ad budget pauses.</p>
          <div class= "mt-6 flex flex-wrap gap-2 font-mono text-[11px] uppercase tracking-widest" >
            < span class= "border border-grape/50 text-grape rounded-full px-3 py-1" > Technical SEO </ span >< span class= "border border-grape/50 text-grape rounded-full px-3 py-1" > Editorial </ span >< span class= "border border-grape/50 text-grape rounded-full px-3 py-1" > Digital PR </ span >< span class= "border border-grape/50 text-grape rounded-full px-3 py-1" > AEO </ span >
          </ div >
        </ div >
        < div class= "md:col-span-5 flex md:justify-end" >
          < div class= "bg-ink/60 border border-grape/30 rounded-2xl p-6 w-full max-w-xs" >
            < div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50" > Organic traffic </ div >
            < div class= "font-display font-extrabold text-6xl text-grape mt-2" > +312 %</ div >
            < div class= "mt-4 flex items-end gap-1 h-14" aria - hidden = "true" >
              < span class= "w-3 bg-grape/40 rounded-sm" style = "height:20%" ></ span >< span class= "w-3 bg-grape/50 rounded-sm" style = "height:32%" ></ span >< span class= "w-3 bg-grape/60 rounded-sm" style = "height:45%" ></ span >< span class= "w-3 bg-grape/70 rounded-sm" style = "height:60%" ></ span >< span class= "w-3 bg-grape/80 rounded-sm" style = "height:78%" ></ span >< span class= "w-3 bg-grape rounded-sm" style = "height:100%" ></ span >
            </ div >
          </ div >
        </ div >
      </ article >
      < !--card 3-- >
      < article class= "np-svc sticky top-[138px] mb-8 rounded-3xl bg-[#1c1410] border border-bone/10 p-8 md:p-14 grid md:grid-cols-12 gap-8 items-center min-h-[62vh]" >
        < div class= "md:col-span-1 font-mono text-mango text-lg" >/ 03 </ div >
        < div class= "md:col-span-6" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > Social & Creator </ h3 >
          < p class= "mt-5 text-bone/60 max-w-lg leading-relaxed" > Always - on social that sounds like a person, not a press release. Creator sourcing, UGC pipelines and community playbooks that turn followers into a distribution army.</p>
          <div class= "mt-6 flex flex-wrap gap-2 font-mono text-[11px] uppercase tracking-widest" >
            < span class= "border border-mango/50 text-mango rounded-full px-3 py-1" > UGC </ span >< span class= "border border-mango/50 text-mango rounded-full px-3 py-1" > Influencer </ span >< span class= "border border-mango/50 text-mango rounded-full px-3 py-1" > Community </ span >< span class= "border border-mango/50 text-mango rounded-full px-3 py-1" > Short - form </ span >
          </ div >
        </ div >
        < div class= "md:col-span-5 flex md:justify-end" >
          < div class= "bg-ink/60 border border-mango/30 rounded-2xl p-6 w-full max-w-xs" >
            < div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50" > Avg.views / launch </ div >
            < div class= "font-display font-extrabold text-6xl text-mango mt-2" > 18M </ div >
            < div class= "mt-4 flex gap-2" aria - hidden = "true" >< span class= "w-8 h-8 rounded-full bg-mango/80" ></ span >< span class= "w-8 h-8 rounded-full bg-mango/60 -ml-4" ></ span >< span class= "w-8 h-8 rounded-full bg-mango/40 -ml-4" ></ span >< span class= "w-8 h-8 rounded-full bg-ink border border-mango/50 -ml-4 font-mono text-[10px] text-mango flex items-center justify-center" > +9k </ span ></ div >
          </ div >
        </ div >
      </ article >
      < !--card 4-- >
      < article class= "np-svc sticky top-[162px] rounded-3xl bg-acid text-ink border border-ink/20 p-8 md:p-14 grid md:grid-cols-12 gap-8 items-center min-h-[62vh]" >
        < div class= "md:col-span-1 font-mono text-ink/70 text-lg" >/ 04 </ div >
        < div class= "md:col-span-6" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > Brand & Creative </ h3 >
          < p class= "mt-5 text-ink/70 max-w-lg leading-relaxed" > Identity, motion and landing pages built to be remembered. A creative studio that ships ad variants at the speed of the feed — and makes your CFO smile at the results.</p>
          <div class= "mt-6 flex flex-wrap gap-2 font-mono text-[11px] uppercase tracking-widest" >
            < span class= "border border-ink/50 rounded-full px-3 py-1" > Identity </ span >< span class= "border border-ink/50 rounded-full px-3 py-1" > Motion </ span >< span class= "border border-ink/50 rounded-full px-3 py-1" > Web Design </ span >< span class= "border border-ink/50 rounded-full px-3 py-1" > Ad Variants </ span >
          </ div >
        </ div >
        < div class= "md:col-span-5 flex md:justify-end" >
          < div class= "bg-ink text-bone rounded-2xl p-6 w-full max-w-xs" >
            < div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50" > Creative shipped / mo </ div >
            < div class= "font-display font-extrabold text-6xl text-acid mt-2" > 120 +</ div >
            < div class= "mt-4 font-mono text-[11px] text-bone/50" > HOOKS • CUTDOWNS • STATIC • 3D • OOH</div>
          </div>
        </div>
      </article>
    </div>
  </div>
</section>

<!-- ============ PROCESS : HORIZONTAL ============ -->
< section id = "process" class= "bg-ink" >
  < div id = "npProcWrap" class= "relative overflow-hidden h-screen flex flex-col justify-center" >
    < div class= "max-w-[1400px] mx-auto px-5 md:px-10 w-full mb-10" >
      < p class= "font-mono text-xs uppercase tracking-[0.3em] text-acid" data - scramble > (03) — The Pulse Method</p>
      <h2 class= "mt-3 font-display font-extrabold uppercase tracking-tight text-4xl md:text-6xl" > Four moves.Zero guesswork.</ h2 >
    </ div >
    < div id = "npProcTrack" class= "flex gap-6 md:gap-10 px-5 md:px-10 w-max" >
      < div class= "w-[85vw] md:w-[42vw] shrink-0 rounded-3xl border border-bone/10 bg-coal p-8 md:p-12 flex flex-col justify-between min-h-[52vh]" >
        < div class= "font-display font-extrabold text-7xl md:text-8xl np-outline-acid" > 01 </ div >
        < div >
          < h3 class= "font-display font-extrabold uppercase text-2xl md:text-3xl" > Discover </ h3 >
          < p class= "mt-4 text-bone/60 leading-relaxed" > Full - funnel audit, data deep-dive and stakeholder interviews. We find the leaks, the levers and the unfair advantages hiding in your numbers.</p>
          <ul class= "mt-6 space-y-2 font-mono text-[12px] text-bone/50 uppercase tracking-widest" >< li >→ Analytics teardown</li><li>→ Market & competitor map</li><li>→ ICP research sprints</li></ul>
        </div>
        <div class= "font-mono text-[11px] text-acid uppercase tracking-widest" > Week 01–02 </ div >
      </ div >
      < div class= "w-[85vw] md:w-[42vw] shrink-0 rounded-3xl border border-bone/10 bg-coal p-8 md:p-12 flex flex-col justify-between min-h-[52vh]" >
        < div class= "font-display font-extrabold text-7xl md:text-8xl np-outline-acid" > 02 </ div >
        < div >
          < h3 class= "font-display font-extrabold uppercase text-2xl md:text-3xl" > Strategy </ h3 >
          < p class= "mt-4 text-bone/60 leading-relaxed" > A 90 - day growth roadmap with positioning, channel mix, budget splits and KPI targets. One page. No fluff. Everyone knows the mission.</p>
          <ul class= "mt-6 space-y-2 font-mono text-[12px] text-bone/50 uppercase tracking-widest" >< li >→ Offer & message architecture </ li >< li >→ Media plan & forecasts</li><li>→ Experiment backlog</li></ul>
        </div>
        <div class= "font-mono text-[11px] text-acid uppercase tracking-widest" > Week 03 </ div >
      </ div >
      < div class= "w-[85vw] md:w-[42vw] shrink-0 rounded-3xl border border-bone/10 bg-coal p-8 md:p-12 flex flex-col justify-between min-h-[52vh]" >
        < div class= "font-display font-extrabold text-7xl md:text-8xl np-outline-acid" > 03 </ div >
        < div >
          < h3 class= "font-display font-extrabold uppercase text-2xl md:text-3xl" > Launch </ h3 >
          < p class= "mt-4 text-bone/60 leading-relaxed" > Creative production, tracking, landing pages and media go live in one coordinated strike. Ship in days, not quarters.</p>
          <ul class= "mt-6 space-y-2 font-mono text-[12px] text-bone/50 uppercase tracking-widest" >< li >→ Ad & UGC production </ li >< li >→ Server - side tracking </ li >< li >→ CRO landing sprints</li></ul>
        </div>
        <div class= "font-mono text-[11px] text-acid uppercase tracking-widest" > Week 04–06 </ div >
      </ div >
      < div class= "w-[85vw] md:w-[42vw] shrink-0 rounded-3xl border border-acid/40 bg-acid text-ink p-8 md:p-12 flex flex-col justify-between min-h-[52vh]" >
        < div class= "font-display font-extrabold text-7xl md:text-8xl" style = "color:transparent;-webkit-text-stroke:1.5px #0A0A0C" > 04 </ div >
        < div >
          < h3 class= "font-display font-extrabold uppercase text-2xl md:text-3xl" > Scale </ h3 >
          < p class= "mt-4 text-ink/70 leading-relaxed" > Weekly test cadence, budget scaling rules and creative iteration. Winners get fed, losers get killed, learnings get compounded.</p>
          <ul class= "mt-6 space-y-2 font-mono text-[12px] text-ink/60 uppercase tracking-widest" >< li >→ 8–12 experiments / mo </ li >< li >→ Incrementality checks</li><li>→ New channel expansion</li></ul>
        </div>
        <div class= "font-mono text-[11px] text-ink uppercase tracking-widest" > Ongoing ∞</ div >
      </ div >
      < div class= "w-[70vw] md:w-[30vw] shrink-0 flex flex-col items-start justify-center gap-6 pr-10" >
        < p class= "font-display font-extrabold uppercase text-3xl md:text-4xl leading-tight" > Ready to run the loop?</p>
        <a href="#contact" data-cursor class= "np-btn-acid inline-flex items-center gap-2 bg-acid text-ink font-mono text-[12px] uppercase tracking-widest font-bold px-7 py-4 rounded-full" > Start with Discover →</a>
      </div>
    </div>
  </div>
</section>

<!-- ============ WORK ============ -->
<section id="work" class= "py-24 md:py-36" >
  < div class= "max-w-[1400px] mx-auto px-5 md:px-10" >
    < div class= "flex flex-wrap items-end justify-between gap-6 mb-16" >
      < div >
        < p class= "font-mono text-xs uppercase tracking-[0.3em] text-acid" data - scramble > (04) — Selected Work</p>
        <h2 class= "mt-4 font-display font-extrabold uppercase tracking-tight text-4xl md:text-6xl leading-none" >
          < span class= "np-rl" >< span > Receipts,</ span ></ span >
          < span class= "np-rl" >< span > not promises.</ span ></ span >
        </ h2 >
      </ div >
      < a href = "#contact" data - cursor class= "np-link font-mono text-xs uppercase tracking-widest text-bone/60 hover:text-acid transition-colors" > All case studies →</ a >
    </ div >

    < div class= "space-y-24" >
      < !-- case 1-- >
      < article class= "np-tilt grid lg:grid-cols-12 gap-8 items-center" data - cursor >
        < div class= "lg:col-span-7 np-kb rounded-3xl overflow-hidden border border-bone/10 relative" >
          < img src = "https://image.qwenlm.ai/public_source/7b1b5bdb-9e22-4a48-82f3-e6d260853da5/13a0867f3-a6db-4c71-9927-e524592873ee.png" alt = "KARTLY fashion campaign" class= "w-full h-[420px] md:h-[560px] object-cover" loading = "lazy" />
          < span class= "absolute top-5 left-5 bg-ink/80 backdrop-blur font-mono text-[11px] uppercase tracking-widest text-acid px-3 py-2 rounded-full" > E - commerce / Fashion </ span >
        </ div >
        < div class= "lg:col-span-5 np-tilt-pop" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > Kartly </ h3 >
          < p class= "mt-4 text-bone/60 leading-relaxed" > A streetwear label stuck at plateau. We rebuilt the funnel — UGC creative engine, CRO sprints and a scaling ladder on Meta & TikTok.</p>
          <div class= "mt-8 grid grid-cols-3 gap-4" >
            < div >< div class= "font-display font-extrabold text-3xl text-acid" > +312 %</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Revenue / 90d </ div ></ div >
            < div >< div class= "font-display font-extrabold text-3xl text-bone" > 5.2x </ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > ROAS </ div ></ div >
            < div >< div class= "font-display font-extrabold text-3xl text-bone" >−44 %</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > CPA </ div ></ div >
          </ div >
          < div class= "mt-8 flex gap-2 font-mono text-[11px] uppercase tracking-widest text-bone/50" >< span class= "border border-bone/20 rounded-full px-3 py-1" > Paid Social </ span >< span class= "border border-bone/20 rounded-full px-3 py-1" > CRO </ span >< span class= "border border-bone/20 rounded-full px-3 py-1" > UGC </ span ></ div >
        </ div >
      </ article >
      < !-- case 2-- >
      < article class= "np-tilt grid lg:grid-cols-12 gap-8 items-center" data - cursor >
        < div class= "lg:col-span-5 order-2 lg:order-1 np-tilt-pop" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > Finlo </ h3 >
          < p class= "mt-4 text-bone/60 leading-relaxed" > A fintech app burning cash on installs. We repositioned the offer, shipped 60 ad variants a month and let cohort data steer the budget.</p>
          <div class= "mt-8 grid grid-cols-3 gap-4" >
            < div >< div class= "font-display font-extrabold text-3xl text-grape" > 2.3M </ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Installs </ div ></ div >
            < div >< div class= "font-display font-extrabold text-3xl text-bone" >−41 %</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > CAC </ div ></ div >
            < div >< div class= "font-display font-extrabold text-3xl text-bone" > +68 %</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > D30 retention </ div ></ div >
          </ div >
          < div class= "mt-8 flex gap-2 font-mono text-[11px] uppercase tracking-widest text-bone/50" >< span class= "border border-bone/20 rounded-full px-3 py-1" > ASO </ span >< span class= "border border-bone/20 rounded-full px-3 py-1" > Performance </ span >< span class= "border border-bone/20 rounded-full px-3 py-1" > Lifecycle </ span ></ div >
        </ div >
        < div class= "lg:col-span-7 order-1 lg:order-2 np-kb rounded-3xl overflow-hidden border border-bone/10 relative" >
          < img src = "https://image.qwenlm.ai/public_source/7b1b5bdb-9e22-4a48-82f3-e6d260853da5/17e46079f-32ad-45c1-9314-26ffcc39331b.png" alt = "Finlo fintech growth campaign" class= "w-full h-[420px] md:h-[560px] object-cover" loading = "lazy" />
          < span class= "absolute top-5 left-5 bg-ink/80 backdrop-blur font-mono text-[11px] uppercase tracking-widest text-grape px-3 py-2 rounded-full" > Fintech / Mobile </ span >
        </ div >
      </ article >
      < !-- case 3-- >
      < article class= "np-tilt grid lg:grid-cols-12 gap-8 items-center" data - cursor >
        < div class= "lg:col-span-7 np-kb rounded-3xl overflow-hidden border border-bone/10 relative" >
          < img src = "https://image.qwenlm.ai/public_source/7b1b5bdb-9e22-4a48-82f3-e6d260853da5/12c0add92-0fd3-4335-8446-e0edae820e55.png" alt = "Brewtree coffee social campaign" class= "w-full h-[420px] md:h-[560px] object-cover" loading = "lazy" />
          < span class= "absolute top-5 left-5 bg-ink/80 backdrop-blur font-mono text-[11px] uppercase tracking-widest text-mango px-3 py-2 rounded-full" > DTC / Coffee </ span >
        </ div >
        < div class= "lg:col-span-5 np-tilt-pop" >
          < h3 class= "font-display font-extrabold uppercase text-3xl md:text-5xl tracking-tight" > Brewtree </ h3 >
          < p class= "mt-4 text-bone/60 leading-relaxed" > A specialty coffee brand with zero social presence. We built a personality-driven content system and an email flow that prints money.</p>
          <div class= "mt-8 grid grid-cols-3 gap-4" >
            < div >< div class= "font-display font-extrabold text-3xl text-mango" > 8.4x </ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Email ROI </ div ></ div >
            < div >< div class= "font-display font-extrabold text-3xl text-bone" > 120K </ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > New followers </ div ></ div >
            < div >< div class= "font-display font-extrabold text-3xl text-bone" > +196 %</ div >< div class= "font-mono text-[11px] uppercase tracking-widest text-bone/50 mt-1" > Subscriptions </ div ></ div >
          </ div >
          < div class= "mt-8 flex gap-2 font-mono text-[11px] uppercase tracking-widest text-bone/50" >< span class= "border border-bone/20 rounded-full px-3 py-1" > Social </ span >< span class= "border border-bone/20 rounded-full px-3 py-1" > Email </ span >< span class= "border border-bone/20 rounded-full px-3 py-1" > Brand </ span ></ div >
        </ div >
      </ article >
    </ div >
  </ div >
</ section >

< !-- ============ IMPACT BAND ============ -->
< section class= "bg-acid text-ink py-20 md:py-28 overflow-hidden" >
  < div class= "max-w-[1400px] mx-auto px-5 md:px-10" >
    < p class= "font-mono text-xs uppercase tracking-[0.3em] text-ink/60" data - scramble > (05) — The scoreboard</p>
    <div class= "mt-10 grid md:grid-cols-4 gap-10" >
      < div >< div class= "font-display font-extrabold text-6xl md:text-7xl tracking-tight" >$< span data - count = "86" > 0 </ span > M </ div >< div class= "font-mono text-[12px] uppercase tracking-widest mt-2 text-ink/60" > Client revenue generated</div></div>
      <div><div class= "font-display font-extrabold text-6xl md:text-7xl tracking-tight" >< span data - count = "1.2" data - decimals = "1" > 0 </ span > B </ div >< div class= "font-mono text-[12px] uppercase tracking-widest mt-2 text-ink/60" > Imppressions engineered </ div ></ div >
      < div >< div class= "font-display font-extrabold text-6xl md:text-7xl tracking-tight" >< span data - count = "4.8" data - decimals = "1" > 0 </ span > x </ div >< div class= "font-mono text-[12px] uppercase tracking-widest mt-2 text-ink/60" > Average blended ROAS</div></div>
      <div><div class= "font-display font-extrabold text-6xl md:text-7xl tracking-tight" >< span data - count = "96" > 0 </ span >%</ div >< div class= "font-mono text-[12px] uppercase tracking-widest mt-2 text-ink/60" > Clients who re-sign</div></div>
    </div>
    <div class= "mt-14 flex items-end gap-1.5 h-28 md:h-36" aria - hidden = "true" >
      < span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:22%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:30%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:26%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:38%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:45%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:41%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:55%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:62%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:58%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:70%" ></ span >< span class= "np-impbar flex-1 bg-ink/85 rounded-t-sm" style = "height:78%" ></ span >< span class= "np-impbar flex-1 bg-ink rounded-t-sm" style = "height:100%" ></ span >
    </ div >
    < div class= "flex justify-between font-mono text-[11px] uppercase tracking-widest text-ink/50 mt-3" >< span > 2019 </ span >< span > 2025 →</ span ></ div >
  </ div >
</ section >

< !-- ============ TESTIMONIALS ============ -->
< section class= "py-24 md:py-36 relative overflow-hidden" >
  < div class= "absolute -left-24 top-10 font-display font-extrabold text-[26rem] leading-none text-bone/5 select-none hidden lg:block" aria - hidden = "true" > "</div>
  < div class= "max-w-[1100px] mx-auto px-5 md:px-10 relative" >
    < p class= "font-mono text-xs uppercase tracking-[0.3em] text-acid text-center" data - scramble > (06) — Word on the street</p>
    <div id="npTwrap" class= "relative mt-12 min-h-[340px] md:min-h-[300px]" >
      < figure class= "np-tslide active text-center" >
        < blockquote class= "font-display font-bold text-2xl md:text-4xl leading-snug tracking-tight" > "Novapulse didn't act like an agency. They acted like a co-founder with a media budget and a grudge against our competitors." </ blockquote >
        < figcaption class= "mt-8 font-mono text-xs uppercase tracking-widest text-bone/50" > Maya Okafor — CMO, <span class= "text-acid" > Kartly </ span ></ figcaption >
      </ figure >
      < figure class= "np-tslide text-center" >
        < blockquote class= "font-display font-bold text-2xl md:text-4xl leading-snug tracking-tight" > "First partner ever to beat our internal forecasts three quarters in a row. The weekly testing cadence is a superpower." </ blockquote >
        < figcaption class= "mt-8 font-mono text-xs uppercase tracking-widest text-bone/50" > Daniel Reyes — VP Growth, <span class= "text-grape" > Finlo </ span ></ figcaption >
      </ figure >
      < figure class= "np-tslide text-center" >
        < blockquote class= "font-display font-bold text-2xl md:text-4xl leading-snug tracking-tight" > "We went from invisible to inevitable. Our email list alone now out-earns what our old agency claimed ads could do." </ blockquote >
        < figcaption class= "mt-8 font-mono text-xs uppercase tracking-widest text-bone/50" > June Park — Founder, <span class= "text-mango" > Brewtree </ span ></ figcaption >
      </ figure >
    </ div >
    < div class= "mt-10 flex items-center justify-center gap-6" >
      < button id = "npTprev" data - cursor aria - label = "Previous testimonial" class= "w-12 h-12 rounded-full border border-bone/25 hover:border-acid hover:text-acid transition-colors" >←</ button >
      < div class= "w-40 h-[2px] bg-bone/15 overflow-hidden" >< div id = "npTprog" class= "h-full bg-acid" ></ div ></ div >
      < button id = "npTnext" data - cursor aria - label = "Next testimonial" class= "w-12 h-12 rounded-full border border-bone/25 hover:border-acid hover:text-acid transition-colors" >→</ button >
    </ div >
  </ div >
</ section >

< !-- ============ PRICING ============ -->
< section id = "pricing" class= "py-24 md:py-36 bg-coal border-y border-bone/10" >
  < div class= "max-w-[1400px] mx-auto px-5 md:px-10" >
    < div class= "text-center mb-16" >
      < p class= "font-mono text-xs uppercase tracking-[0.3em] text-acid" data - scramble > (07) — Engagements </ p >
      < h2 class= "mt-4 font-display font-extrabold uppercase tracking-tight text-4xl md:text-6xl" >
        < span class= "np-rl" >< span > Pick your velocity</span></span>
      </h2>
      <p class= "mt-4 text-bone/55 max-w-xl mx-auto np-fade" > Month - to - month after the first 90 days. If we don't beat your baseline, we work free until we do. That's the pulse guarantee.</p>
    </div>

    <div class= "grid lg:grid-cols-3 gap-6 items-stretch" >
      < div class= "np-fade rounded-3xl border border-bone/15 p-8 md:p-10 flex flex-col hover:-translate-y-2 hover:border-bone/40 transition-all duration-500" >
        < div class= "font-mono text-xs uppercase tracking-widest text-bone/50" > Ignition </ div >
        < div class= "mt-4 font-display font-extrabold text-5xl" >$1.9k < span class= "text-lg text-bone/40 font-body font-normal" >/ mo </ span ></ div >
        < p class= "mt-4 text-bone/55 text-sm leading-relaxed" > For early - stage brands finding their first repeatable channel.</p>
        <ul class= "mt-8 space-y-3 text-sm text-bone/70 flex-1" >
          < li class= "flex gap-3" >< span class= "text-acid" >✓</ span > 1 paid channel management</li>
          <li class= "flex gap-3" >< span class= "text-acid" >✓</ span > 8 ad creatives / month</li>
          <li class= "flex gap-3" >< span class= "text-acid" >✓</ span > Landing page CRO basics</li>
          <li class= "flex gap-3" >< span class= "text-acid" >✓</ span > Monthly strategy call</li>
          <li class= "flex gap-3" >< span class= "text-acid" >✓</ span > Live reporting dashboard</li>
        </ul>
        <a href = "#contact" data-cursor class= "mt-10 text-center border border-bone/25 hover:border-acid hover:text-acid transition-colors rounded-full py-4 font-mono text-xs uppercase tracking-widest" > Ignite →</ a >
      </ div >

      < div class= "np-fade relative rounded-3xl bg-acid text-ink p-8 md:p-10 flex flex-col lg:-translate-y-4 shadow-[0_30px_90px_rgba(201,247,58,.25)]" >
        < span class= "absolute -top-4 left-1/2 -translate-x-1/2 bg-ink text-acid font-mono text-[11px] uppercase tracking-widest px-4 py-2 rounded-full" > Most booked </ span >
        < div class= "font-mono text-xs uppercase tracking-widest text-ink/60" > Velocity </ div >
        < div class= "mt-4 font-display font-extrabold text-5xl" >$4.5k < span class= "text-lg text-ink/50 font-body font-normal" >/ mo </ span ></ div >
        < p class= "mt-4 text-ink/70 text-sm leading-relaxed" > For scaling brands ready to turn marketing into a machine.</p>
        <ul class= "mt-8 space-y-3 text-sm font-medium flex-1" >
          < li class= "flex gap-3" >< span >✓</ span > 3 channels + full - funnel CRO </ li >
          < li class= "flex gap-3" >< span >✓</ span > 24 creatives + 6 UGC videos / mo</li>
          <li class= "flex gap-3" >< span >✓</ span > Email & lifecycle flows </ li >
          < li class= "flex gap-3" >< span >✓</ span > Weekly testing sprints</li>
          <li class= "flex gap-3" >< span >✓</ span > Dedicated growth lead</li>
          <li class= "flex gap-3" >< span >✓</ span > Slack access, 4h response</li>
        </ul>
        <a href="#contact" data-cursor class= "mt-10 text-center bg-ink text-acid rounded-full py-4 font-mono text-xs uppercase tracking-widest hover:bg-bone hover:text-ink transition-colors" > Accelerate →</ a >
      </ div >

      < div class= "np-fade rounded-3xl border border-bone/15 p-8 md:p-10 flex flex-col hover:-translate-y-2 hover:border-bone/40 transition-all duration-500" >
        < div class= "font-mono text-xs uppercase tracking-widest text-bone/50" > Domination </ div >
        < div class= "mt-4 font-display font-extrabold text-5xl" >$9k < span class= "text-lg text-bone/40 font-body font-normal" > +/ mo </ span ></ div >
        < p class= "mt-4 text-bone/55 text-sm leading-relaxed" > For market leaders who want the category to feel inevitable.</p>
        <ul class= "mt-8 space-y-3 text-sm text-bone/70 flex-1" >
          < li class= "flex gap-3" >< span class= "text-acid" >✓</ span > Everything in Velocity </ li >
          < li class= "flex gap-3" >< span class= "text-acid" >✓</ span > Embedded squad(strategist + 3) </ li >
          < li class= "flex gap-3" >< span class= "text-acid" >✓</ span > Brand & motion studio access</li>
          <li class= "flex gap-3" >< span class= "text-acid" >✓</span> MMM + incrementality testing</li>
          <li class="flex gap-3"><span class="text-acid">✓</span> Quarterly executive offsites</li>
        </ul>
        <a href="#contact" data-cursor class="mt-10 text-center border border-bone/25 hover:border-acid hover:text-acid transition-colors rounded-full py-4 font-mono text-xs uppercase tracking-widest">Dominate →</a>
      </div>
    </div>
  </div>
</section>

<!-- ============ FAQ ============ -->
<section id="faq" class="py-24 md:py-36">
  <div class="max-w-[1400px] mx-auto px-5 md:px-10 grid lg:grid-cols-12 gap-12">
    <div class="lg:col-span-4">
      <p class="font-mono text-xs uppercase tracking-[0.3em] text-acid" data-scramble>(08) — Straight answers</p>
      <h2 class="mt-4 font-display font-extrabold uppercase tracking-tight text-4xl md:text-5xl leading-tight">
        <span class="np-rl"><span>Asked.</span></span>
        <span class="np-rl"><span>Answered.</span></span>
      </h2>
      <p class="mt-6 text-bone/55 np-fade">Still curious? Ping us at <a href="mailto:hello@novapulse.agency" class="text-acid np-link" data-cursor>hello@novapulse.agency</a> — a human replies within a day.</p>
    </div>
    <div class="lg:col-span-8">
      <div class="np-faq-item border-t border-bone/15 py-6">
        <button class="np-faq-q w-full flex items-center justify-between gap-6 text-left" data-cursor>
          <span class="font-display font-bold text-lg md:text-2xl">How fast until we see results?</span><span class="np-faq-x text-acid text-2xl font-mono shrink-0">+</span>
        </button>
        <div class="np-faq-a"><p class="pt-4 text-bone/60 leading-relaxed max-w-2xl">Leading indicators (CTR, CPA, landing CVR) move within the first 2–3 weeks. Compounding revenue results typically show between day 45 and 90, once the testing engine has enough signal to scale winners.</p></div>
      </div>
      <div class="np-faq-item border-t border-bone/15 py-6">
        <button class="np-faq-q w-full flex items-center justify-between gap-6 text-left" data-cursor>
          <span class="font-display font-bold text-lg md:text-2xl">Do you work with our in-house team?</span><span class="np-faq-x text-acid text-2xl font-mono shrink-0">+</span>
        </button>
        <div class="np-faq-a"><p class="pt-4 text-bone/60 leading-relaxed max-w-2xl">Love to. About half our accounts are hybrid — we plug into your Slack, share the same dashboards and upskill your team as we go. No black boxes, ever.</p></div>
      </div>
      <div class="np-faq-item border-t border-bone/15 py-6">
        <button class="np-faq-q w-full flex items-center justify-between gap-6 text-left" data-cursor>
          <span class="font-display font-bold text-lg md:text-2xl">What ad spend do we need?</span><span class="np-faq-x text-acid text-2xl font-mono shrink-0">+</span>
        </button>
        <div class="np-faq-a"><p class="pt-4 text-bone/60 leading-relaxed max-w-2xl">We recommend at least $10k/month in media for paid engagements — below that, the testing loop starves. For SEO & content, meaningful programs start around $5k/month.</p></div>
      </div>
      <div class="np-faq-item border-t border-bone/15 py-6">
        <button class="np-faq-q w-full flex items-center justify-between gap-6 text-left" data-cursor>
          <span class="font-display font-bold text-lg md:text-2xl">Who actually works on our account?</span><span class="np-faq-x text-acid text-2xl font-mono shrink-0">+</span>
        </button>
        <div class="np-faq-a"><p class="pt-4 text-bone/60 leading-relaxed max-w-2xl">The people you meet on the first call. A dedicated growth lead plus specialists in media, creative and CRO. We cap accounts per lead so nobody juggles more than four brands.</p></div>
      </div>
      <div class="np-faq-item border-t border-b border-bone/15 py-6">
        <button class="np-faq-q w-full flex items-center justify-between gap-6 text-left" data-cursor>
          <span class="font-display font-bold text-lg md:text-2xl">What's the pulse guarantee?</span><span class="np-faq-x text-acid text-2xl font-mono shrink-0">+</span>
        </button>
        <div class="np-faq-a"><p class="pt-4 text-bone/60 leading-relaxed max-w-2xl">Simple: if we don't beat your agreed baseline KPIs inside the first 90 days, we keep working for free until we do. We've paid out exactly twice in twelve years.</p></div>
      </div>
    </div>
  </div>
</section>

<!-- ============ CTA ============ -->
<section id="contact" class="relative py-28 md:py-40 overflow-hidden">
  <div class="absolute inset-0 np-gridbg" aria-hidden="true"></div>
  <div class="np-marquee absolute top-10 left-0 right-0 opacity-[0.12] pointer-events-none" aria-hidden="true">
    <div class="np-mtrack font-display font-extrabold uppercase text-[9vw] leading-none whitespace-nowrap" style="--mdur:20s">
      <div class="flex shrink-0"><span class="px-8">Let's grow</span><span class="text-acid">→</span><span class="px-8">Let's grow</span><span class="text-acid">→</span><span class="px-8">Let's grow</span><span class="text-acid">→</span></div>
      <div class="flex shrink-0"><span class="px-8">Let's grow</span><span class="text-acid">→</span><span class="px-8">Let's grow</span><span class="text-acid">→</span><span class="px-8">Let's grow</span><span class="text-acid">→</span></div>
    </div>
  </div>

  <div class="relative max-w-[900px] mx-auto px-5 md:px-10 text-center">
    <p class="font-mono text-xs uppercase tracking-[0.3em] text-acid" data-scramble>(09) — Your move</p>
    <h2 class="mt-6 font-display font-extrabold uppercase tracking-tight text-5xl md:text-8xl leading-[0.95]">
      <span class="np-rl"><span>Stop renting</span></span>
      <span class="np-rl"><span class="text-acid">attention.</span></span>
      <span class="np-rl"><span>Own it.</span></span>
    </h2>
    <p class="mt-8 text-bone/60 max-w-lg mx-auto np-fade">Book a free 30-minute strategy call. You'll leave with three growth levers you can use — even if you never hire us.</p>

    <form id="npForm" class="mt-10 flex flex-col sm:flex-row gap-3 max-w-xl mx-auto">
      <input id="npEmail" type="email" required placeholder="you@yourbrand.com" data-cursor class="flex-1 bg-coal border border-bone/20 focus:border-acid outline-none rounded-full px-6 py-4 font-mono text-sm placeholder:text-bone/30 transition-colors"/>
      <button type="submit" data-cursor data-magnetic class="np-btn-acid bg-acid text-ink font-mono text-[12px] uppercase tracking-widest font-bold px-8 py-4 rounded-full">Get my growth plan →</button>
    </form>
    <p id="npFormMsg" class="mt-4 font-mono text-xs text-acid h-5" aria-live="polite"></p>
  </div>
</section>

</main>

<!-- ============ FOOTER ============ -->
<footer class="bg-coal border-t border-bone/10 pt-16 pb-8">
  <div class="max-w-[1400px] mx-auto px-5 md:px-10">
    <div class="grid md:grid-cols-12 gap-10">
      <div class="md:col-span-5">
        <a href="#top" class="flex items-center gap-2 w-max" data-cursor>
          <svg width="28" height="28" viewBox="0 0 32 32" fill="none"><rect x="1" y="1" width="30" height="30" rx="8" stroke="#C9F73A" stroke-width="2"/><path d="M8 20 L13 20 L15 12 L18 22 L20 15 L24 15" stroke="#C9F73A" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round"/></svg>
          <span class="font-display font-extrabold text-lg">NOVA<span class="text-acid">PULSE</span>®</span>
        </a>
        <p class="mt-5 text-bone/50 text-sm max-w-xs leading-relaxed">The digital growth studio for brands that refuse to be ignored. Attention in, revenue out.</p>
        <div class="mt-6 flex gap-3">
          <a href="#" data-cursor aria-label="Instagram" class="w-11 h-11 rounded-full border border-bone/20 hover:border-acid hover:text-acid hover:-translate-y-1 transition-all flex items-center justify-center font-mono text-xs">IG</a>
          <a href="#" data-cursor aria-label="LinkedIn" class="w-11 h-11 rounded-full border border-bone/20 hover:border-acid hover:text-acid hover:-translate-y-1 transition-all flex items-center justify-center font-mono text-xs">LI</a>
          <a href="#" data-cursor aria-label="TikTok" class="w-11 h-11 rounded-full border border-bone/20 hover:border-acid hover:text-acid hover:-translate-y-1 transition-all flex items-center justify-center font-mono text-xs">TT</a>
          <a href="#" data-cursor aria-label="X" class="w-11 h-11 rounded-full border border-bone/20 hover:border-acid hover:text-acid hover:-translate-y-1 transition-all flex items-center justify-center font-mono text-xs">X</a>
        </div>
      </div>
      <div class="md:col-span-2">
        <div class="font-mono text-[11px] uppercase tracking-widest text-bone/40 mb-5">Sitemap</div>
        <ul class="space-y-3 text-sm text-bone/70">
          <li><a class="hover:text-acid transition-colors np-link" href="#services" data-cursor>Services</a></li>
          <li><a class="hover:text-acid transition-colors np-link" href="#process" data-cursor>Process</a></li>
          <li><a class="hover:text-acid transition-colors np-link" href="#work" data-cursor>Work</a></li>
          <li><a class="hover:text-acid transition-colors np-link" href="#pricing" data-cursor>Pricing</a></li>
        </ul>
      </div>
      <div class="md:col-span-3">
        <div class="font-mono text-[11px] uppercase tracking-widest text-bone/40 mb-5">Contact</div>
        <ul class="space-y-3 text-sm text-bone/70">
          <li><a class="hover:text-acid transition-colors" href="mailto:hello@novapulse.agency" data-cursor>hello@novapulse.agency</a></li>
          <li><a class="hover:text-acid transition-colors" href="tel:+12125550148" data-cursor>+1 (212) 555-0148</a></li>
          <li class="text-bone/40">404 Greene St, NYC / 12 Shoreditch, LDN</li>
        </ul>
      </div>
      <div class="md:col-span-2">
        <div class="font-mono text-[11px] uppercase tracking-widest text-bone/40 mb-5">Studio time</div>
        <div class="font-mono text-2xl text-acid np-clock">00:00:00</div>
        <div class="font-mono text-[11px] text-bone/40 mt-1 uppercase tracking-widest">New York — HQ</div>
      </div>
    </div>

    <div class="mt-14 overflow-hidden" aria-hidden="true">
      <div class="font-display font-extrabold uppercase leading-none text-center text-[13.5vw] tracking-tight np-outline opacity-30 select-none">Novapulse</div>
    </div>

    <div class="mt-8 pt-6 border-t border-bone/10 flex flex-col md:flex-row items-center justify-between gap-4 font-mono text-[11px] uppercase tracking-widest text-bone/40">
      <span>© 2025 Novapulse Studio LLC — All rights reserved</span>
      <span>Made with caffeine & conversion rates</span>
      <button id="npTop" data-cursor class="hover:text-acid transition-colors">Back to top ↑</button>
    </div>
  </div>
</footer>

<script src="https://cdn.jsdelivr.net/npm/gsap@3.12.5/dist/gsap.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/gsap@3.12.5/dist/ScrollTrigger.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/lenis@1.1.13/dist/lenis.min.js"></script>
<script>
(function(){
  'use strict';
  gsap.registerPlugin(ScrollTrigger);
  var REDUCED = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  var FINE = window.matchMedia('(hover:hover) and (pointer:fine)').matches;

  /* ---------- smooth scroll (lenis) ---------- */
  var lenis = null;
  if (!REDUCED && window.Lenis){
    lenis = new Lenis({ lerp:0.1, smoothWheel:true });
    lenis.on('scroll', ScrollTrigger.update);
    gsap.ticker.add(function(t){ lenis.raf(t*1000); });
    gsap.ticker.lagSmoothing(0);
  }
  function npScrollTo(sel){
    if (lenis){ lenis.scrollTo(sel, { offset:-70, duration:1.4 }); }
    else { var el=document.querySelector(sel); if(el) el.scrollIntoView(); }
  }
  document.querySelectorAll('a[href^="#"]').forEach(function(a){
    a.addEventListener('click', function(e){
      var id=a.getAttribute('href');
      if(id.length>1 && document.querySelector(id)){ e.preventDefault(); npCloseMenu(); npScrollTo(id); }
    });
  });

  /* ---------- clock ---------- */
  function tickClock(){
    var d=new Date();
    var s=String(d.getHours()).padStart(2,'0')+':'+String(d.getMinutes()).padStart(2,'0')+':'+String(d.getSeconds()).padStart(2,'0');
    document.querySelectorAll('.np-clock').forEach(function(el){ el.textContent=s; });
  }
  tickClock(); setInterval(tickClock,1000);

  /* ---------- scramble ---------- */
  var NP_CHARS='ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#$%&<>/=*';
  function npScramble(el,text,dur){
    if(REDUCED){ el.textContent=text; return; }
    dur=dur||700;
    var total=Math.max(8,Math.round(dur/30)), frame=0;
    var timer=setInterval(function(){
      frame++;
      var prog=frame/total;
      el.textContent=text.split('').map(function(c,i){
        if(c===' ')return ' ';
        return (i/text.length<prog)? c : NP_CHARS[Math.floor(Math.random()*NP_CHARS.length)];
      }).join('');
      if(frame>=total){ clearInterval(timer); el.textContent=text; }
    },30);
  }

  /* ---------- rotating hero word ---------- */
  var NP_WORDS=['scrolls','clicks','likes','searches','views'];
  var npWi=0, npRot=document.getElementById('npRotWord');
  if(npRot && !REDUCED){
    setInterval(function(){ npWi=(npWi+1)%NP_WORDS.length; npScramble(npRot,NP_WORDS[npWi],600); },2600);
  }

  /* ---------- loader ---------- */
  var loader=document.getElementById('npLoader');
  function startSite(){
    var tl=gsap.timeline();
    tl.fromTo('#npLoader .np-lword i',{yPercent:110},{yPercent:0,duration:.7,stagger:.05,ease:'power4.out'})
      .to('#npLoadBar',{width:'100%',duration:1.2,ease:'power2.inOut'},0)
      .to({v:0},{v:100,duration:1.2,ease:'power2.inOut',onUpdate:function(){ document.getElementById('npLoadPct').textContent=String(Math.round(this.targets()[0].v)).padStart(3,'0'); }},0)
      .to(loader,{yPercent:-100,duration:.9,ease:'power4.inOut'})
      .add(function(){ loader.style.display='none'; heroIntro(); ScrollTrigger.refresh(); });
    if(REDUCED){ tl.duration(0.01); }
  }
  function heroIntro(){
    if(REDUCED){
      document.querySelectorAll('.np-rl > span').forEach(function(s){ s.style.transform='none'; });
      return;
    }
    var tl=gsap.timeline({defaults:{ease:'power4.out'}});
    tl.to('#npHeroMeta',{opacity:1,y:0,duration:.8,from:{opacity:0,y:20}},0)
      .to('.np-rl > span',{y:0,duration:1.1,stagger:.12},0.1)
      .fromTo('#npHeroSub',{opacity:0,y:30},{opacity:1,y:0,duration:.9},0.6)
      .fromTo('#npHeroCtas',{opacity:0,y:30},{opacity:1,y:0,duration:.9},0.75)
      .fromTo('#npHeroCard',{opacity:0,scale:.9,y:40},{opacity:1,scale:1,y:0,duration:1},0.7);
  }
  startSite();

  /* ---------- header state ---------- */
  var header=document.getElementById('npHeader');
  window.addEventListener('scroll',function(){ header.classList.toggle('scrolled',window.scrollY>40); },{passive:true});

  /* ---------- mobile menu ---------- */
  var menu=document.getElementById('npMenu');
  function npCloseMenu(){ menu.classList.remove('open'); }
  document.getElementById('npBurger').addEventListener('click',function(){ menu.classList.add('open'); });
  document.getElementById('npClose').addEventListener('click',npCloseMenu);
  menu.querySelectorAll('a').forEach(function(a){ a.addEventListener('click',npCloseMenu); });

  /* ---------- scroll progress ---------- */
  gsap.to('#npProgress',{scaleX:1,ease:'none',scrollTrigger:{trigger:document.body,start:'top top',end:'bottom bottom',scrub:.3}});

  /* ---------- scramble eyebrows on enter ---------- */
  document.querySelectorAll('[data-scramble]').forEach(function(el){
    var txt=el.textContent;
    ScrollTrigger.create({trigger:el,start:'top 88%',once:true,onEnter:function(){ npScramble(el,txt,800); }});
  });

  /* ---------- line reveals & fades ---------- */
  if(!REDUCED){
    gsap.utils.toArray('.np-rl > span').forEach(function(s){
      gsap.to(s,{y:0,duration:1,ease:'power4.out',scrollTrigger:{trigger:s.parentElement,start:'top 90%'}});
    });
    gsap.utils.toArray('.np-fade').forEach(function(el){
      gsap.fromTo(el,{opacity:0,y:36},{opacity:1,y:0,duration:.9,ease:'power3.out',scrollTrigger:{trigger:el,start:'top 90%'}});
    });
  } else {
    document.querySelectorAll('.np-rl > span').forEach(function(s){ s.style.transform='none'; });
  }

  /* ---------- counters ---------- */
  document.querySelectorAll('[data-count]').forEach(function(el){
    var target=parseFloat(el.getAttribute('data-count'));
    var dec=parseInt(el.getAttribute('data-decimals')||'0',10);
    ScrollTrigger.create({trigger:el,start:'top 90%',once:true,onEnter:function(){
      if(REDUCED){ el.textContent=target.toFixed(dec); return; }
      var obj={v:0};
      gsap.to(obj,{v:target,duration:2,ease:'power2.out',onUpdate:function(){ el.textContent=obj.v.toFixed(dec); }});
    }});
  });

  /* ---------- impact bars ---------- */
  if(!REDUCED){
    gsap.to('.np-impbar',{scaleY:1,duration:1.1,stagger:.07,ease:'power3.out',scrollTrigger:{trigger:'.np-impbar',start:'top 92%'}});
  } else {
    document.querySelectorAll('.np-impbar').forEach(function(b){ b.style.transform='scaleY(1)'; });
  }

  /* ---------- stacked service cards ---------- */
  var svcCards=document.querySelectorAll('.np-svc');
  svcCards.forEach(function(card,i){
    if(i<svcCards.length-1 && !REDUCED){
      gsap.to(card,{scale:.93,opacity:.55,ease:'none',scrollTrigger:{trigger:svcCards[i+1],start:'top bottom',end:'top top',scrub:true}});
    }
  });

  /* ---------- horizontal process ---------- */
  var procWrap=document.getElementById('npProcWrap');
  var procTrack=document.getElementById('npProcTrack');
  function procDist(){ return Math.max(0, procTrack.scrollWidth - procWrap.offsetWidth); }
  if(procWrap && procTrack && !REDUCED){
    gsap.to(procTrack,{x:function(){ return -procDist(); },ease:'none',scrollTrigger:{trigger:procWrap,start:'top top',end:function(){ return '+='+procDist(); },scrub:1,pin:true,invalidateOnRefresh:true,anticipatePin:1}});
  } else if (procWrap){ procWrap.style.height='auto'; procTrack.style.flexWrap='wrap'; procTrack.style.width='100%'; }

  /* ---------- tilt on case cards ---------- */
  if(FINE && !REDUCED){
    document.querySelectorAll('.np-tilt').forEach(function(card){
      card.addEventListener('mousemove',function(e){
        var r=card.getBoundingClientRect();
        var px=(e.clientX-r.left)/r.width-.5, py=(e.clientY-r.top)/r.height-.5;
        gsap.to(card,{rotationY:px*6,rotationX:-py*6,transformPerspective:900,duration:.5});
      });
      card.addEventListener('mouseleave',function(){ gsap.to(card,{rotationY:0,rotationX:0,duration:.7}); });
    });
  }

  /* ---------- testimonials rotator ---------- */
  var tSlides=document.querySelectorAll('.np-tslide');
  var tIdx=0, tTimer=null;
  var tProg=document.getElementById('npTprog');
  function tShow(i){
    tIdx=(i+tSlides.length)%tSlides.length;
    tSlides.forEach(function(s,j){ s.classList.toggle('active',j===tIdx); });
    if(!REDUCED && tProg){ gsap.fromTo(tProg,{scaleX:0},{scaleX:1,duration:6,ease:'none',overwrite:true}); }
  }
  function tAuto(){ if(REDUCED)return; clearInterval(tTimer); tTimer=setInterval(function(){ tShow(tIdx+1); },6000); }
  document.getElementById('npTnext').addEventListener('click',function(){ tShow(tIdx+1); tAuto(); });
  document.getElementById('npTprev').addEventListener('click',function(){ tShow(tIdx-1); tAuto(); });
  tShow(0); tAuto();

  /* ---------- faq ---------- */
  document.querySelectorAll('.np-faq-q').forEach(function(btn){
    btn.addEventListener('click',function(){
      var item=btn.parentElement, ans=item.querySelector('.np-faq-a');
      var open=item.classList.toggle('open');
      ans.style.maxHeight=open? ans.scrollHeight+'px':'0px';
    });
  });

  /* ---------- magnetic buttons ---------- */
  if(FINE && !REDUCED){
    document.querySelectorAll('[data-magnetic]').forEach(function(b){
      b.addEventListener('mousemove',function(e){
        var r=b.getBoundingClientRect();
        gsap.to(b,{x:(e.clientX-r.left-r.width/2)*.35,y:(e.clientY-r.top-r.height/2)*.35,duration:.4});
      });
      b.addEventListener('mouseleave',function(){ gsap.to(b,{x:0,y:0,duration:.6,ease:'elastic.out(1,.4)'}); });
    });
  }

  /* ---------- custom cursor ---------- */
  if(FINE && !REDUCED){
    var dot=document.getElementById('npDot'), ring=document.getElementById('npRing');
    var mx=innerWidth/2,my=innerHeight/2;
    var dx=gsap.quickTo(dot,'left',{duration:.12}), dy=gsap.quickTo(dot,'top',{duration:.12});
    var rx=gsap.quickTo(ring,'left',{duration:.4}), ry=gsap.quickTo(ring,'top',{duration:.4});
    window.addEventListener('mousemove',function(e){ mx=e.clientX;my=e.clientY; dx(mx);dy(my);rx(mx);ry(my); });
    document.querySelectorAll('a,button,[data-cursor]').forEach(function(el){
      el.addEventListener('mouseenter',function(){ document.body.classList.add('np-cur-hover'); });
      el.addEventListener('mouseleave',function(){ document.body.classList.remove('np-cur-hover'); });
    });
  } else { dot.style.display='none'; ring.style.display='none'; }

  /* ---------- form ---------- */
  document.getElementById('npForm').addEventListener('submit',function(e){
    e.preventDefault();
    var msg=document.getElementById('npFormMsg');
    npScramble(msg,'✓ RECEIVED — YOUR GROWTH PLAN LANDS WITHIN 24H.',900);
    document.getElementById('npEmail').value='';
  });

  /* ---------- back to top ---------- */
  document.getElementById('npTop').addEventListener('click',function(){
    if(lenis){ lenis.scrollTo(0,{duration:1.6}); } else { window.scrollTo({top:0,behavior:REDUCED?'auto':'smooth'}); }
  });

  window.addEventListener('load',function(){ ScrollTrigger.refresh(); });
})();
</script>
</body>
</html>

"""
    }
}
