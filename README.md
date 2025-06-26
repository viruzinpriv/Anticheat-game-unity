# 🛡️ AntiCheat Unity Mobile + Firebase

Sistema completo de detecção de trapaças em jogos desenvolvidos com **Unity para Android**, com integração em tempo real com o **Firebase Realtime Database**.

---

## 🚀 Funcionalidades

- ✅ Detecção de **SpeedHack**
- ✅ Detecção de **Time Manipulation**
- ✅ Comparação de memória falsa (**Memory Hack**)
- ✅ Verificação de **Score Hack** comparando com Firebase
- ✅ Verificação de **Root**
- ✅ Checagem de integridade do **APK via SHA-256**
- ✅ Detecção de apps proibidos (GameGuardian, LuckyPatcher, etc)
- ✅ Envio de relatórios para o Firebase
- ✅ Penalidade automática (fechar o jogo)

---

## 🔧 Requisitos

- Unity **2023 LTS** ou superior
- Firebase Unity SDK instalado:
  - Auth
  - Database
- Ativar login anônimo no Firebase
- Adicionar permissão no `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.QUERY_ALL_PACKAGES" />
```

---

## 📦 Estrutura Firebase

O script envia relatórios para o caminho:

```
/cheaters/{userId}/
  └── auto_id: {
        motivo: "SpeedHack",
        horario: "2025-06-26 13:00:00",
        dispositivo: "SM-A146M"
      }
```

---

## 🧠 Como usar

1. Importe o script `AntiCheatManager.cs` no seu projeto
2. Insira em um GameObject na cena inicial
3. Substitua `originalHash` com o SHA-256 real do seu APK
4. Compile o APK e teste com as tentativas de trapaça
5. Os relatórios aparecerão no Firebase

---

## 📸 Painel Web (opcional)

Você pode visualizar os cheaters em tempo real com um painel em HTML conectado ao Firebase.  
Peça o painel aqui ou [veja um exemplo neste repositório](#).

---

## 🧑‍💻 Autor

Desenvolvido por **Solut3**  
🌐 Canal, conteúdo técnico, scripts e segurança ofensiva

---

## 📄 Licença

Distribuído sob a Licença MIT. Veja `LICENSE` para mais informações.

---

## ❤️ Contribuições

Sugestões, melhorias ou detecções adicionais são bem-vindas!  
Abra uma issue ou envie um pull request.
