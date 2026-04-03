# 🎯 Vertical Slice — Shattered Skies (Multiplayer Local)

## 🧠 1. Loop Principal (ESSENCIAL)

Objetivo: provar que o jogo funciona como uma experiência completa.

* Spawn dos 2 aviões
* Tempo de preparação (2–3 segundos)
* Combate começa
* Um jogador é destruído
* Tela de vitória aparece
* Opção de replay

---

## ✈️ 2. Sistema de Combate Básico

### 🔫 Tiro

* Disparo simples (metralhadora)
* Cadência controlada
* Som + muzzle flash

### 💥 Dano

* Vida simples (ex: 3 hits ou barra pequena)
* Feedback ao tomar dano:

  * Tela pisca
  * Som
  * Leve screen shake

### 💣 Explosão

* Explosão estilizada ao morrer
* Avião perde controle (cai girando)
* Delay antes de desaparecer

---

## 🧍‍♂️🧍‍♂️ 3. Multiplayer Local

* Input separado para cada player
* Spawn em lados opostos
* Câmera independente (split-screen)

### 💡 Dica

* Manter jogadores relativamente próximos
* Evitar mapa grande demais

---

## 🗺️ 4. Arena Simples

* Céu aberto
* 2–3 elementos:

  * Nuvens (visual ou cover leve)
  * Ilhas flutuantes ou mar com navio
* Limite invisível

---

## 🎮 5. Game Feel

* Screen shake leve ao atirar/acertar
* Sons impactantes (tiro, hit, explosão)
* Partículas:

  * Fumaça
  * Faísca ao acertar
* Rastro no avião

---

## 🏆 6. Condição de Vitória + Feedback

* Freeze curto (≈0.5s)
* Texto grande: "PLAYER 1 WINS"
* Som de vitória
* Opções:

  * Revanche
  * Voltar

---

## 🧪 7. Polimento Mínimo

* UI básica (vida dos jogadores)
* Identificação (P1 / P2)
* Fade in no início
* Fade out na vitória

---

## ⚡ Ordem de Implementação

1. Loop completo (spawn → fight → win → restart)
2. Tiro + dano
3. Morte + explosão
4. Multiplayer estável
5. Arena simples
6. Game feel
7. UI + vitória

---

## 🧠 Mentalidade

Não focar em:

* Sistema complexo de armas
* Progressão
* Menus elaborados

Foco:

> O jogo é divertido em 5 minutos com 2 jogadores?

---

## 💡 Ideias Extras (Opcional)

* Boost (aceleração temporária)
* Stamina de curva
* Bala limitada
* Avião soltando fumaça com pouca vida
