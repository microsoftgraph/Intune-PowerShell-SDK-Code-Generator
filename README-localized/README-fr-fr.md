# Table des matières
- [Table des matières](#table-of-contents)
- [Intune-PowerShell-SDK-Code-Generator](#intune-powershell-sdk-code-generator)
- [Contribution](#contributing)
- [Conception](#design)
    - [Architecture de haut niveau](#high-level-architecture)
    - [Étape de génération automatique](#the-auto-generation-step)
- [Travail futur](#future-work)
- [FAQ](#faq)
    - [Où se trouve le code généré ?](#where-is-the-generated-code)
    - [Pourquoi ce module PowerShell est-il nécessaire ?](#why-do-we-need-this-powershell-module)
    - [Pourquoi générer automatiquement le module PowerShell ?](#why-do-we-want-to-auto-generate-the-powershell-module)
    - [Pourquoi est-il élaboré par Intune plutôt que par l’équipe du kit de développement logiciel (SDK) Graph ?](#why-is-intune-building-it-and-why-isnt-the-graph-sdk-team-building-it)
    - [Présentation de Vipr](#what-is-vipr)
    - [Pourquoi utiliser un rédacteur personnalisé ?](#why-a-custom-writer)
    - [Pourquoi ne pas utiliser OpenAPI et Swagger ?](#why-not-use-openapi-and-swagger)

# Intune-PowerShell-SDK-Code-Generator
Ce référentiel implémente le rédacteur Vipr qui permet de générer des cmdlets PowerShell pour toutes les opérations, fonctions et actions CRUD pour la prise en charge de l’API Microsoft Intune Graph.

# Contribuer
Ce projet autorise les contributions et les suggestions.
Pour la plupart des contributions, vous devez accepter le contrat de licence de contributeur (CLA, Contributor License Agreement) stipulant que vous êtes en mesure, et que vous vous y engagez, de nous accorder les droits d’utiliser votre contribution.
Pour plus d’informations, visitez https://cla.microsoft.com.

Lorsque vous soumettez une requête de tirage, un robot CLA détermine automatiquement si vous devez fournir un CLA et si vous devez remplir la requête de tirage appropriée (par exemple, étiquette, commentaire).
Suivez simplement les instructions données par le robot.
Vous ne devrez le faire qu’une seule fois au sein de tous les référentiels à l’aide du CLA.

Ce projet a adopté le [code de conduite Open Source de Microsoft](https://opensource.microsoft.com/codeofconduct/).
Pour en savoir plus, reportez-vous à la [FAQ relative au code de conduite](https://opensource.microsoft.com/codeofconduct/faq/)
ou contactez [opencode@microsoft.com](mailto:opencode@microsoft.com) pour toute question ou tout commentaire.

# Conception
## Architecture de haut niveau
![Architecture de haut niveau](Design.jpg)
Voici comment s’articule ce module PowerShell.
1. Vipr.exe est exécuté avec les arguments suivants pour générer le code qui définit le comportement des cmdlets :
– La sortie de génération du projet GraphODataPowerShellWriter (c-à-d : GraphODataPowerShellWriter.dll).
– Un schéma Graph (ex : ce que renvoie https://graph.microsoft.com/v1.0/$metadata) ; l’extension de ce fichier est « .csdl », il est situé dans « ~/TestGraphSchemas ».
2. La documentation générée comme donnée de sortie de Vipr est extraite vers un fichier XML. Ce fichier XML est ensuite utilisé par PowerShell pour afficher le résultat de la cmdlet « Get-Help ».
3. Il est plus cohérent de taper certaines fonctionnalités manuellement dans PowerShell. Ces cmdlets sont donc écrites dans leurs propres modules. Ces modules sont situés dans « ~/src/PowerShellGraphSDK/PowerShellModuleAdditions/CustomModules ».
4. Les résultats des trois étapes précédentes (c-à-d : les cmdlets générées, la documentation des cmdlets et les cmdlets manuelles) sont combinés selon un fichier manifeste PowerShell. L’extension de ce fichier est « .psd1 ».
5. Le fichier manifeste PowerShell créé à l’étape précédente est ensuite utilisé pour importer et utiliser le module. Par exemple, avec un fichier manifeste « Intune.psd1 », exécutez « Import-Module ./Intune.psd1 » dans une fenêtre PowerShell pour importer le module.

## Étape de génération automatique
![Génération du module binaire PowerShell](Generating_the_PowerShell_Binary_Module.jpg)
Ceci est un aperçu détaillé de l’étape 1 de la section [Architecture de haut niveau](#high-level-architecture).
– Le fichier de schéma de définition (CSDL) est passé à Vipr.
– Le lecteur OData v4 intégré à Vipr est utilisé pour lire ce fichier. Ce lecteur convertit la définition de schéma en une représentation intermédiaire appelée Modèle ODCM.
– Notre rédacteur personnalisé est passé à Vipr pour le traiter. Le rédacteur effectue 4 étapes afin de convertir le modèle ODCM et générer les fichiers C# finaux :
1. Parcours du Modèle ODCM (c-à-d le schéma) pour découvrir chaque route.
2. Pour chaque route, création d’une représentation abstraite des cmdlets représentant chaque opération de la route.
3. Conversion de chaque représentation abstraite de cmdlet en une représentation abstraite C#.
4. Pour chaque représentation abstraite C#, conversion en une chaîne ensuite écrite comme contenu d’un fichier.

# Travail futur
- [] Améliorer le nommage des cmdlets générées.
    - [] Ajouter si possible des annotations au schéma Graph pour la création de noms compréhensibles pour les routes.
- [] Trouver un moyen de générer et intégrer des cmdlets pour le schéma Graph entier dans un délai raisonnable.
    - Le nombre de routes augmente énormément lorsque des entités AAD sont incluses (en raison des propriétés de navigation).
- [] Faire en sorte que la documentation d’aide de la cmdlet générée inclue un lien à la page appropriée de la documentation Graph officielle.
- [x] Implémenter la canalisation des sorties vers d’autres cmdlets directement.
- [] Implémenter l’authentification non-interactive.
    - [] Authentification avec certificats.
    - [x] Authentification avec un objet PSCredential.
- [x] S’assurer d’obtenir la date d’expiration correcte du jeton d’authentification – Possible en rafraîchissant automatiquement le jeton quand c’est nécessaire.
- [x] Créer un référentiel séparé pour les cmdlets de scénario (à fin d’utilisation comme sous-module).
- [] Intégration Travis CI GitHub + cas de tests.
- [x] Créer un fichier de licence pour les logiciels tiers utilisés et l’inclure dans le module.
- [x] Mettre à jour le fichier \*.psd1 pour inclure les liens corrects vers le GitHub publique, le fichier de licence et l’icône.
- [x] Créer des cmdlets d’assistance pour créer des objets pouvant être sérialisés dans des types définis dans le schéma.
- [] Ajouter des Restrictions de Capabilité OData au schéma Graph afin que les cmdlets effectuant des opérations interdites ne soient tout simplement pas générées.

# FAQ
## Où se trouve le code généré ?
GitHub : https://github.com/Microsoft/Intune-PowerShell-SDK

## Pourquoi ce module PowerShell est-il nécessaire ?
Les clients ont exprimé un grand intérêt dans l’utilisation de PowerShell pour automatiser des tâches actuellement exécutées par le biais de l’extension Intune dans le Portail Azure.

## Pourquoi générer automatiquement le module PowerShell ?
En tapant chaque cmdlet manuellement, il faut mettre à jour le module manuellement chaque fois que le schéma Graph est mis à jour.

## Pourquoi est-il élaboré par Intune plutôt que par l’équipe du kit de développement logiciel (SDK) Graph ?
La plus grande part de la base d’utilisateurs d’Intune (IT Professionals), préfèrent travailler avec PowerShell, alors que le kit de développement logiciel Graph vise les développeurs qui préfèrent travailler avec, par exemple, C#, Java et Python. Voilà pourquoi nous avons urgemment besoin de ce module. La structure syntaxique de PowerShell ne se prête pas à celle du générateur du kit de développement logiciel .Net/Java/Python (voir pourquoi nous avons un [rédacteur Vipr personnalisé](#why-a-custom-writer)).

## Présentation de Vipr
- L’architecture est construite autour du projet de recherche Microsoft Vipr.
- Vipr généralise le concept de transformation d’informations d’une syntaxe à une autre.
- Vipr est comme un analyseur de texte modulaire spécialement adapté aux définitions des services dans le style d’OData.
- L’exécutable accepte un assembly lecteur et un assembly rédacteur.
    - Le lecteur peut analyser le fichier texte d’entrée et le transformer en une représentation (générique) intermédiaire.
    - Le rédacteur reçoit cette représentation intermédiaire et renvoie une liste d’objets fichiers, par exemple des paires (chaîne, chemin d’accès).
        - Vipr écrit ensuite ces fichiers de la structure du dossier donné au dossier de sortie.

## Pourquoi utiliser un rédacteur personnalisé ?
Il n’existe aucun rédacteur PowerShell pour Vipr. De même, le rédacteur du kit de développement logiciel Graph pour .Net/Java/Python ne se prête pas facilement à la syntaxe, le comportement ou les bonnes habitudes de PowerShell.

## Pourquoi ne pas utiliser OpenAPI et Swagger ?
C’est une possibilité, mais cela nécessiterait une transformation supplémentaire du schéma OData de Graph vers le format OpenAPI. À chaque transformation, il y a un risque de perte d’informations précieuses dans le module PowerShell généré.