# ⚡ FPS Boost Pro

Activez le plein potentiel de votre machine. **FPS Boost Pro** est une application moderne développée en **C# / WPF** (architecture MVVM) permettant d'optimiser Windows 11 pour le gaming et les tâches intensives.

---

## 🚀 Fonctionnalités principales

* **🏠 Dashboard interactif** : Visualisez l'état global de votre système en un coup d'œil.
* **⚡ One Click Boost** : Crée automatiquement un point de restauration système avant d'appliquer les optimisations majeures.
* **🌐 Network Tweaks** : Optimisation des paramètres de la carte réseau (latence, DNS, paquets) via scripts PowerShell sécurisés.
* **⚙ Services Manager** : Désactivation intelligente des services superflus et de la télémétrie Windows.
* **🔋 Power Management** : Activation de profils d'alimentation hautes performances cachés de Windows.
* **🎮 Gaming Tweaks** : Optimisations avancées du Registre (optimisation FSE, désactivation Game DVR, etc.).

---

## 🛠 Technologies & Architecture

Cette application a été conçue en respectant les meilleures pratiques de développement et d'administration système Windows :

* **Langage** : C# (.NET 8)
* **Interface** : XAML (WPF) avec un design sombre et épuré.
* **Architecture** : MVVM (implémenté via le *Community Toolkit MVVM*).
* **Sécurité** : Privilèges Administrateur requis (gérés via `app.manifest`) pour modifier le Registre et exécuter les commandes système.

---

## 📸 Aperçu de l'interface

> 💡 *Astuce : Glissez-déposez vos captures d'écran du projet ici pour les afficher !*

| Onglet Principal | Tweaks Système |
| :---: | :---: |
| ![Dashboard](https://via.placeholder.com/400x250?text=Apercu+Dashboard) | ![Tweaks](https://via.placeholder.com/400x250?text=Apercu+Tweaks) |

---

## ⚠️ Sécurité & Prérequis

Toutes les optimisations agissent directement sur le cœur du système d'exploitation.

1. **Droits d'administrateur requis** : L'application demande automatiquement une élévation de privilèges (UAC) au lancement.
2. **Point de restauration** : Fortement recommandé et intégré directement dans l'onglet *One Click Boost*.

---

## ✒️ Développeur

* **Diae** - *Créateur & Administrateur Système* - [Mon GitHub](https://github.com/DiCR77)
