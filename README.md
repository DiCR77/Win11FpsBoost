#                                                            -------------------     ** ⚡ DiFPSBoost (Windows 11)**     --------------------

<img width="1024" height="1024" alt="image" src="https://github.com/user-attachments/assets/2db2ebe8-39b7-433d-bbe7-d4b4f81ed817" />

Activez le plein potentiel de votre machine ! **DiFPSBoost** est une application moderne développée en **C# / WPF** (architecture MVVM) permettant d'optimiser Windows 11 pour le gaming, de réduire la latence (ping) et de nettoyer le système en toute sécurité.

---
🛡️ Workflow Sécurisé : "D'abord la sécurité"
Nous avons repensé l'expérience utilisateur pour garantir une optimisation sans risque. Le tableau de bord privilégie désormais un workflow en deux étapes :

Point de Restauration (Priorité) : L'application intègre la création automatique de points de restauration système (Checkpoint-Computer) avant toute modification.

Optimisation en un clic : Une fois la sécurité garantie, lancez le nettoyage et les tweaks système en toute sérénité.

---
## 🚀 Fonctionnalités principales

* **🏠 Tableau de bord interactif** : Visualisez l'état global de votre système en un coup d'œil.
* **⚡ Boost en un clic** : Nettoyage en profondeur des caches et fichiers temporaires (%TEMP%) couplé à une protection par point de restauration.
* **💻 Console d'Audit en Direct** : Suivez chaque action étape par étape grâce à une console intégrée interactive. Elle affiche en temps réel les réussites et détaille les erreurs (ex: droits administrateur, échecs de commandes système).
* **🌐 Optimisation Réseau (Ping)** : Réduction de la latence via la désactivation du Network Throttling, l'activation du Receive Side Scaling (RSS), le vidage DNS et l'optimisation TCP Autotuning.
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
## 📊 Résultats de Benchmark (Avant / Après)

Test de performance réel effectué sur le jeu **Unturned** avec un processeur **AMD Ryzen 7 5700U** (Radeon Graphics intégrée) :

* **FPS Moyens (Average FPS)** : De **50.1 FPS** ➔ **61.7 FPS** (**+23%** de performances brutes)
* **1% Percentile FPS (Stabilité)** : De **29.6 FPS** ➔ **43.7 FPS** (**+47%** de fluidité globale)
* **0.2% Percentile FPS (Saccades)** : De **16.8 FPS** ➔ **35.9 FPS** (**+113%** de réduction des drops de FPS)

> *Grâce aux optimisations de registre, d'alimentation et de gestion des services de DiFPSBoost, les micro-saccades (stuttering) ont été éradiquées, offrant un gameplay parfaitement fluide.*
<img width="1058" height="360" alt="image" src="https://github.com/user-attachments/assets/b5b695a0-b172-4305-b4ca-481eec288520" />

---
## 📸 Aperçu de l'interface

* **Tableau de bord & Console d'Audit**

 <img width="1167" height="951" alt="image" src="https://github.com/user-attachments/assets/4ad35981-9977-471a-8f1f-0b8e3ca0dcdc" />
 
---

## ⚠️ Sécurité & VM / Sandbox

Toutes les optimisations agissent directement sur le cœur du système d'exploitation.

* **Droits d'administrateur requis** : L'application demande automatiquement une élévation de privilèges (UAC) au lancement.
* **Point de restauration** : Fortement recommandé et intégré directement dans l'onglet *One Click Boost*.
* **Note de test (VM / Sandbox)** : Certaines fonctions de bas niveau (comme le *TRIM SSD* ou la *Création de Point de Restauration*) peuvent afficher un échec logique dans les environnements virtuels bridés (Windows Sandbox, VirtualBox) car ces fonctionnalités y sont désactivées par Windows lui-même.

---

## ✒️ Développeur

* **Diae** - *Créateur & Administrateur Système* - [Mon GitHub](https://github.com/ton-pseudo)
