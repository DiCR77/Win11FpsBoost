# ⚡ DiFPSBoost (Windows 11)

Activez le plein potentiel de votre machine ! **FPSBoostPro** est une application moderne développée en **C# / WPF** (architecture MVVM) permettant d'optimiser Windows 11 pour le gaming, de réduire la latence (ping) et de nettoyer le système en toute sécurité.

---

## 🚀 Fonctionnalités principales

* **🏠 Tableau de bord interactif** : Visualisez l'état global de votre système en un coup d'œil.
* **⚡ Boost en un clic** : Crée automatiquement un point de restauration système avant de nettoyer en profondeur les caches et les fichiers temporaires (`%TEMP%`).
* **💻 Console d'Audit en Direct** : Suivez chaque action étape par étape grâce à une console intégrée interactive qui affiche les réussites et détaille les erreurs (ex: droits administrateur manquants).
* **🌐 Optimisation Réseau (Ping)** : Réduction de la latence via la désactivation du *Network Throttling*, l'activation du *Receive Side Scaling (RSS)*, le vidage DNS et l'optimisation TCP Autotuning.
* **⚙️ Services Manager** : Désactivation intelligente et asynchrone des services superflus et de la télémétrie Windows (*DiagTrack*, *WSearch*, *SysMain*).
* **🔋 Alimentation Avancée** : Déblocage et activation du profil caché **Performances Optimales** de Windows, désactivation de la mise en veille des ports USB, du PCI Express (LSPM) et des disques durs.
* **🎮 Tweaks Gaming** : Activation du *Windows Game Mode*, désactivation de la latence du *Game DVR* et de l'accélération de la souris pour une précision $1:1$.

---

## 🛠 Technologies & Architecture

Cette application a été conçue en respectant les meilleures pratiques de développement applicatif et d'administration système :

* **Langage** : C# (.NET 8 / .NET 9)
* **Interface** : XAML (WPF) avec un design sombre, moderne et épuré.
* **Architecture** : MVVM (implémenté de manière robuste via le *Community Toolkit MVVM*).
* **Asynchronisme** : Toutes les tâches système lourdes utilisent `async/await` et s'exécutent en arrière-plan pour éviter de figer l'interface (No-Freeze UI).
* **Sécurité** : Privilèges Administrateur requis (gérés de manière transparente via le fichier `app.manifest`) pour modifier le Registre et exécuter les commandes système.

---

## 📸 Aperçu de l'interface

* Onglet Principal (Dashboard)
<img width="1120" height="653" alt="image" src="https://github.com/user-attachments/assets/af1cf65f-d11c-442e-b83b-52e0fba31be1" />
* <img width="841" height="624" alt="image" src="https://github.com/user-attachments/assets/e461c08b-3106-4955-ac41-bef37e3a798f" />
* <img width="814" height="609" alt="image" src="https://github.com/user-attachments/assets/d9568a89-9387-4751-8fb2-1e03643d2c89" />


---

## ⚠️ Sécurité & VM / Sandbox

Toutes les optimisations agissent directement sur le cœur du système d'exploitation.

* **Droits d'administrateur requis** : L'application demande automatiquement une élévation de privilèges (UAC) au lancement.
* **Point de restauration** : Fortement recommandé et intégré directement dans l'onglet *One Click Boost*.
* **Note de test (VM / Sandbox)** : Certaines fonctions de bas niveau (comme le *TRIM SSD* ou la *Création de Point de Restauration*) peuvent afficher un échec logique dans les environnements virtuels bridés (Windows Sandbox, VirtualBox) car ces fonctionnalités y sont désactivées par Windows lui-même.

---

## ✒️ Développeur

* **Diae** - *Créateur & Administrateur Système* - [Mon GitHub](https://github.com/ton-pseudo)
