# ⚡ Di FPS Boost

Activez le plein potentiel de votre machine. **DiFpsBoost** est une application moderne développée en **C# / WPF** (architecture MVVM) permettant d'optimiser Windows 11 pour le gaming et les tâches intensives.

---

## 🚀 Fonctionnalités principales

* **🏠 Tableau de bord interactif** : Visualisez l'état global de votre système en un coup d'œil.
* **⚡ Boost en un clic** : Crée automatiquement un point de restauration système avant d'appliquer les optimisations majeures.
* **🌐 Réseau** : Optimisation des paramètres de la carte réseau (latence, DNS, paquets) via scripts PowerShell sécurisés.
* **⚙ Services Manager** : Désactivation intelligente des services superflus et de la télémétrie Windows.
* **🔋 Alimentation** : Activation de profils d'alimentation hautes performances cachés de Windows.
* **🎮 Optimisations Gaming** : Optimisations avancées du Registre (optimisation FSE, désactivation Game DVR, etc.).

---

## 🛠 Technologies & Architecture

Cette application a été conçue en respectant les meilleures pratiques de développement et d'administration système Windows :

* **Langage** : C# (.NET 8)
* **Interface** : XAML (WPF) avec un design sombre et épuré.
* **Architecture** : MVVM (implémenté via le *Community Toolkit MVVM*).
* **Sécurité** : Privilèges Administrateur requis (gérés via `app.manifest`) pour modifier le Registre et exécuter les commandes système.

---

## 📸 Aperçu de l'interface

| Onglet Principal | Tweaks Système |
| :---: | :---: |
| **Dashboard**<img width="1120" height="653" alt="image" src="https://github.com/user-attachments/assets/af1cf65f-d11c-442e-b83b-52e0fba31be1" />| **Gaming**<img width="692" height="349" alt="image" src="https://github.com/user-attachments/assets/2d184cb6-b16f-4050-a504-8a8be403e6d9" />|

---

## ⚠️ Sécurité & Prérequis

Toutes les optimisations agissent directement sur le cœur du système d'exploitation.

1. **Droits d'administrateur requis** : L'application demande automatiquement une élévation de privilèges (UAC) au lancement.
2. **Point de restauration** : Fortement recommandé et intégré directement dans l'onglet *One Click Boost*.

---

## ✒️ Développeur

* **Diae** - *Créateur & Administrateur Système* - [Mon GitHub](https://github.com/DiCR77)
