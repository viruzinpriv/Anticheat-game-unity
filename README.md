# Anticheat-game-unity

> 🛡️ Sistema Anti-Cheat para Unity (Mobile)

Este repositório contém um sistema completo de anti-cheat desenvolvido para Unity (Android), com as seguintes proteções:

Verificação de SpeedHack (Time.timeScale)

Detecção de manipulação de tempo (Time.deltaTime)

Detecção de alterações de memória

Comparação de pontuação falsa com Firebase

Verificação de Root

Verificação de apps proibidos (GameGuardian, LuckyPatcher, etc)

Verificação de integridade do APK via SHA-256

Envio de reports para Firebase Realtime Database

Aplicação de penalidades (fechamento do app)


✅ Requisitos:

Unity 2023+

Firebase Unity SDK (Auth + Database)

Permissão no AndroidManifest:


<uses-permission android:name="android.permission.QUERY_ALL_PACKAGES" />

🔐 Os relatórios são armazenados em /cheaters/{userId} com motivo, horário e dispositivo.

👨‍💻 Desenvolvido por Solut3
