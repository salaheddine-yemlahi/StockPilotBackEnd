# StockPilot --- Documentation technique

> **Projet portfolio :** application de gestion de stock et de commandes
> pour petites entreprises.\
> **Backend :** ASP.NET Core Web API / .NET 10\
> **Base de données de production :** PostgreSQL sur AlwaysData\
> **Frontend prévu :** Angular\
> **Langue :** français

------------------------------------------------------------------------

## 1. Présentation du projet

StockPilot est une application de gestion de stock destinée à
centraliser les produits, les catégories et les mouvements de stock. Le
projet a été développé comme une application en couches afin de séparer
les responsabilités et de faciliter les tests et la maintenance.

Les fonctionnalités prévues ou présentes dans le backend comprennent
notamment :

-   la gestion des produits et de leurs références SKU ;
-   la gestion des catégories ;
-   le suivi des quantités en stock et des seuils minimums ;
-   les mouvements de stock ;
-   des informations de tableau de bord ;
-   une authentification basée sur JWT et des rôles applicatifs ;
-   la préparation d'une interface Angular qui consommera l'API.

> **Note sur l'état du projet :** les tests unitaires décrits plus bas
> ont été exécutés avec succès. La documentation ne prétend pas que des
> tests d'intégration automatisés couvrant HTTP et une vraie base de
> données ont déjà été réalisés : cette couverture doit être ajoutée et
> vérifiée séparément.

## 2. Architecture du backend

La solution Visual Studio est organisée en projets distincts :

``` text
StockPilotBackEnd/
├── StockPilot.API/
├── StockPilot.Application/
├── StockPilot.Domain/
├── StockPilot.Infrastructure/
├── StockPilot.Tests/
└── StockPilotBackEnd.slnx
```

### `StockPilot.API`

Point d'entrée de l'application. Ce projet héberge l'API HTTP, la
configuration, l'injection de dépendances, les middlewares et les
contrôleurs/endpoints. Il est également responsable de la configuration
de l'authentification JWT.

### `StockPilot.Application`

Contient la logique applicative et les cas d'utilisation. Cette couche
orchestre les opérations métier sans devoir dépendre directement des
détails de la base de données.

### `StockPilot.Domain`

Contient les entités et les concepts métier du domaine, par exemple les
produits, catégories et mouvements de stock.

### `StockPilot.Infrastructure`

Contient les détails techniques d'accès aux données et les
implémentations des dépendances externes, notamment Entity Framework
Core et les fournisseurs de base de données.

### `StockPilot.Tests`

Contient les tests automatisés. Les tests unitaires du service produit
utilisent un faux dépôt (fake repository) en mémoire pour isoler la
logique du service.

### Pourquoi cette séparation ?

-   **Maintenabilité :** chaque couche a une responsabilité
    identifiable.
-   **Testabilité :** la logique métier peut être testée sans démarrer
    l'API ni contacter PostgreSQL.
-   **Évolutivité :** l'accès aux données peut évoluer sans réécrire
    toute la logique métier.
-   **Lisibilité :** les responsabilités HTTP, métier et persistance ne
    sont pas mélangées.

## 3. Technologies utilisées

-   C# et ASP.NET Core Web API avec .NET 10 ;
-   Entity Framework Core 10 ;
-   SQL Server utilisé pour le développement local à une étape du projet
    ;
-   PostgreSQL pour la base de données hébergée sur AlwaysData ;
-   ASP.NET Core Identity pour les utilisateurs et les rôles, selon la
    configuration du projet ;
-   JWT Bearer pour protéger les endpoints ;
-   xUnit pour les tests automatisés ;
-   Visual Studio pour le développement backend ;
-   Postman pour tester les endpoints HTTP ;
-   Git pour le versionnement ;
-   Angular comme frontend prévu pour consommer l'API.

## 4. Modèle de données

La base PostgreSQL de production contient notamment les tables suivantes
:

-   `Categories`
-   `Products`
-   `StockMovements`
-   les tables d'Identity, dont `AspNetUsers` et `AspNetRoles`

La table `Products` comporte notamment les colonnes suivantes :

  Colonne               Rôle
  --------------------- ----------------------------------------------
  `Id`                  Identifiant du produit
  `Name`                Nom du produit
  `SKU`                 Référence unique ou code de stock du produit
  `Description`         Description
  `UnitPrice`           Prix unitaire
  `QuantityInStock`     Quantité disponible
  `MinimumStockLevel`   Seuil minimum déclenchant une alerte
  `CreatedAt`           Date de création
  `CategoryId`          Référence vers la catégorie

La table `Categories` contient notamment un identifiant, un nom, une
description et une date de création. Les relations exactes et les
contraintes doivent être vérifiées dans les configurations EF Core du
projet.

### Données de démonstration

Des catégories et des produits de démonstration ont été ajoutés
directement à PostgreSQL via un éditeur SQL connecté à la base
AlwaysData. La requête d'insertion des produits :

-   fournit `CreatedAt` avec `NOW()` car la colonne est obligatoire ;
-   associe chaque produit à sa catégorie en recherchant
    `Categories.Name` ;
-   vérifie l'existence du SKU avant insertion pour éviter de réinsérer
    les références déjà présentes ;
-   utilise le vrai nom de colonne `"SKU"` (majuscules), car PostgreSQL
    distingue les identifiants entre guillemets.

Exemple de vérification :

``` sql
SELECT COUNT(*) AS total
FROM public."Products";
```

Pour vérifier les colonnes réelles d'une table PostgreSQL :

``` sql
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'Products'
ORDER BY ordinal_position;
```

## 5. Authentification et autorisation JWT

### Principe

L'API utilise des JSON Web Tokens (JWT) pour authentifier les appels
HTTP. Après une authentification réussie, le backend génère un token
signé. Le client doit ensuite envoyer ce token dans l'en-tête HTTP
`Authorization` pour appeler les endpoints protégés.

``` http
Authorization: Bearer <JWT>
```

Le token testé pendant le développement contenait notamment :

-   un identifiant utilisateur (`sub` / NameIdentifier) ;
-   l'adresse e-mail de l'utilisateur ;
-   un identifiant de token (`jti`) ;
-   un rôle applicatif (`Employee`) ;
-   une date d'émission et une date d'expiration ;
-   un issuer et une audience.

Les valeurs d'issuer et d'audience observées dans le token étaient
`StockPilot.API` et `StockPilot.Client`. Elles doivent correspondre aux
valeurs utilisées par la configuration de validation JWT dans l'API.

### Configuration

La configuration de production est fournie par les variables
d'environnement AlwaysData, plutôt que par des secrets enregistrés dans
Git :

``` text
Jwt__Issuer=StockPilot.API
Jwt__Audience=StockPilot.Client
Jwt__SecretKey=<CLE_SECRETE_LONGUE_ET_ALEATOIRE>
```

Dans la configuration .NET, le double underscore `__` représente le
séparateur `:`. Par exemple, `Jwt__Issuer` correspond à `Jwt:Issuer`.

La chaîne de connexion à la base est également fournie par variable
d'environnement :

``` text
ConnectionStrings__DefaultConnection=Host=<HOTE>;Port=5432;Database=<BASE>;Username=<UTILISATEUR>;Password=<MOT_DE_PASSE>
```

**Ne jamais committer de vrais mots de passe, de clés JWT, de tokens ou
de chaînes de connexion contenant des identifiants.** Les valeurs
ci-dessus sont des modèles à compléter localement ou dans l'interface de
configuration de l'hébergeur.

### Erreur rencontrée et résolution

Lors de la première tentative de génération du token, l'API a répondu :

``` text
L'Issuer JWT est absent de la configuration.
```

Cela indiquait que l'application ne trouvait pas l'issuer attendu dans
sa configuration au moment de générer le token. La configuration
d'hébergement a été complétée avec les variables JWT, notamment
`Jwt__Issuer` et `Jwt__Audience`, puis l'application a été redémarrée.
Une nouvelle authentification a ensuite retourné un JWT.

> Les noms exacts des routes d'inscription/connexion et les champs JSON
> doivent être vérifiés dans le contrôleur d'authentification ou les DTO
> du code source. Ne pas supposer que des routes d'exemple correspondent
> automatiquement à l'implémentation.

### Tester l'authentification dans Postman

1.  Envoyer une requête HTTP à l'endpoint d'inscription ou de connexion
    réellement défini dans le backend.
2.  Fournir le corps JSON attendu par le DTO correspondant.
3.  Récupérer le JWT retourné par l'API.
4.  Pour tester une route protégée, choisir **Authorization → Bearer
    Token** dans Postman et coller le token.
5.  Envoyer une requête, par exemple `GET /api/products`, sur l'URL de
    production.

Interprétation habituelle des réponses :

-   `200 OK` : requête traitée avec succès ;
-   `400 Bad Request` : corps ou données invalides ;
-   `401 Unauthorized` : token absent, invalide, expiré ou configuration
    JWT incohérente ;
-   `403 Forbidden` : identité authentifiée mais autorisation
    insuffisante ;
-   `404 Not Found` : route non disponible à l'URL demandée.

Le endpoint de produits a été testé en production : sans token, il
renvoyait `401`; avec le JWT obtenu, la requête a fonctionné.

### Rôles

Le token testé portait le rôle `Employee`. Les autorisations effectives
dépendent des attributs de contrôleur, des policies et de la
configuration de rôles dans le code. Il faut vérifier ces règles dans le
backend avant de documenter des permissions précises pour `Admin` ou
`Employee`.

## 6. Tests automatisés

### 6.1 Tests unitaires existants

Le projet contient des tests xUnit pour le service de gestion des
produits. Le service est testé à l'aide d'un faux dépôt en mémoire, ce
qui permet d'isoler la logique métier sans se connecter à SQL Server ou
PostgreSQL.

Les six scénarios confirmés sont :

  Scénario                               Résultat attendu
  -------------------------------------- -----------------------------
  Création d'un produit valide           Le produit est créé
  Création avec un SKU déjà utilisé      Une exception est levée
  Mise à jour d'un produit existant      Le produit est mis à jour
  Mise à jour d'un identifiant inconnu   Le service retourne `null`
  Suppression d'un produit existant      La suppression réussit
  Suppression d'un identifiant inconnu   Le service retourne `false`

Ces tests vérifient les règles du service produit sans lancer le serveur
web et sans dépendre d'une vraie base de données. Les tests xUnit ont
été exécutés et confirmés comme réussis pendant le développement.

### Lancer les tests

Depuis le dossier contenant `StockPilotBackEnd.slnx`, ouvrir un terminal
:

``` bash
dotnet test
```

Pour cibler explicitement le projet de tests :

``` bash
dotnet test StockPilot.Tests/StockPilot.Tests.csproj
```

Pour afficher davantage de détails :

``` bash
dotnet test --verbosity normal
```

Si la solution ou les chemins de projets diffèrent dans le dépôt cloné,
adapter le chemin au fichier `.slnx` ou `.csproj` présent.

### 6.2 Limites de la couverture actuelle

Les tests unitaires du service produit ne prouvent pas à eux seuls que
les contrôleurs HTTP, l'authentification JWT, le mapping JSON, Entity
Framework Core ou la connexion PostgreSQL fonctionnent ensemble.

À la date de cette documentation, aucun test d'intégration automatisé
précis n'a été confirmé comme exécuté. Les tests réalisés avec Postman
sur l'API déployée sont des tests manuels d'API, pas des tests
d'intégration automatisés xUnit.

### 6.3 Tests d'intégration recommandés

Pour compléter la couverture, ajouter des tests d'intégration qui
démarrent l'application en environnement de test et envoient des
requêtes HTTP à l'API. Avec ASP.NET Core,
`Microsoft.AspNetCore.Mvc.Testing` et `WebApplicationFactory<Program>`
sont une approche courante.

Les scénarios recommandés :

1.  `GET /api/products` sans token : vérifier `401 Unauthorized` si la
    route exige une authentification.
2.  `GET /api/products` avec un token valide : vérifier le code de
    succès et le format de la réponse.
3.  Requête avec un token expiré ou invalide : vérifier le refus.
4.  Accès avec un rôle insuffisant : vérifier `403 Forbidden` si une
    policy l'exige.
5.  Création d'un produit valide : vérifier la réponse et les données
    persistées.
6.  Création avec un SKU dupliqué : vérifier le comportement prévu par
    l'API.
7.  Référence à une catégorie inexistante : vérifier la validation
    attendue.
8.  Vérifier que les tests ne modifient jamais la base de production.

Pour tester la persistance, utiliser une base dédiée aux tests ou un
fournisseur de test adapté. Ne jamais lancer des tests destructifs
contre la base de production AlwaysData.

## 7. Déploiement de l'API sur AlwaysData

### 7.1 Base de données PostgreSQL

Une base PostgreSQL a été créée sur AlwaysData et la configuration de
connexion a été transmise à l'application par variable d'environnement.

Format de la chaîne de connexion :

``` text
Host=<HOTE_POSTGRESQL>;Port=5432;Database=<NOM_BASE>;Username=<UTILISATEUR>;Password=<SECRET>
```

Les secrets réels doivent rester dans les paramètres de l'hébergement.
Ils ne doivent apparaître ni dans ce document ni dans le dépôt Git.

Les migrations PostgreSQL ont été préparées à l'aide d'un contexte
adapté à PostgreSQL (`PostgreSqlDbContext`) puis appliquées à la base
hébergée. Cette distinction était nécessaire car le développement local
avait initialement utilisé SQL Server : les migrations et types SQL ne
sont pas toujours interchangeables entre fournisseurs EF Core.

### 7.2 Publication de l'API

L'API a été publiée pour exécution sur AlwaysData. Le répertoire de
travail configuré était :

``` text
/home/salah/www/StockPilot_API
```

La commande de démarrage configurée était de la forme :

``` bash
dotnet StockPilot.API.dll --urls "http://$IP:$PORT"
```

AlwaysData fournit les valeurs `$IP` et `$PORT` à l'application.
L'application écoute donc le port fourni par l'hébergeur plutôt qu'un
port local codé en dur.

### 7.3 Domaine et DNS

Le domaine personnalisé configuré pour l'API est :

``` text
stockpilot-api.yemlahi.org
```

Un enregistrement DNS de type A a été configuré pour le sous-domaine
`stockpilot-api`, pointant vers l'adresse IP fournie par AlwaysData. La
propagation DNS a été testée : un résolveur public pouvait résoudre le
nom alors que le résolveur local renvoyait encore `NXDOMAIN`. Cela peut
arriver pendant la propagation ou à cause du cache DNS local.

### 7.4 Vérifications effectuées

Les vérifications suivantes ont été réalisées :

-   l'application démarre sur AlwaysData ;
-   les journaux montrent des requêtes à PostgreSQL et aux tables
    Identity ;
-   le domaine atteint le serveur de l'application ;
-   une requête vers `/` retourne `404`, ce qui peut être normal si
    aucun endpoint racine n'est défini ;
-   l'endpoint des produits répond `401` sans token, ce qui confirme
    qu'il est protégé ;
-   après correction de la configuration JWT, l'authentification a
    retourné un token ;
-   la requête de produits avec ce token a fonctionné.

Un avertissement relatif au port HTTPS de redirection a été observé dans
les logs. Il ne bloquait pas le démarrage de l'application. Il faut
néanmoins vérifier la configuration des redirections HTTPS et du proxy
de l'hébergeur pour s'assurer que le comportement est correct en
production.

### 7.5 Configuration de production

Exemple de noms de variables à configurer dans AlwaysData :

``` text
ConnectionStrings__DefaultConnection=<CHAINE_DE_CONNEXION_POSTGRESQL>
Jwt__Issuer=StockPilot.API
Jwt__Audience=StockPilot.Client
Jwt__SecretKey=<CLE_SECRETE_LONGUE_ET_ALEATOIRE>
```

Les valeurs d'issuer et d'audience doivent être cohérentes avec le code
de génération et de validation des JWT. La clé secrète doit être
suffisamment longue, générée aléatoirement et gardée confidentielle.

Après une modification des variables d'environnement, redémarrer
l'application pour que la configuration soit rechargée.

### 7.6 Dépannage rencontré

  ------------------------------------------------------------------------------------------------------------------
  Problème                                         Cause ou explication    Solution appliquée ou vérification
  ------------------------------------------------ ----------------------- -----------------------------------------
  L'API n'arrive pas à joindre PostgreSQL          Chaîne de connexion ou  Vérifier la variable
                                                   configuration réseau    `ConnectionStrings__DefaultConnection`,
                                                   incorrecte              l'hôte, le port et les identifiants

  Avertissement de redirection HTTPS               Le port HTTPS n'est pas Vérifier la configuration HTTPS et le
                                                   déterminé par           proxy ; l'avertissement observé
                                                   l'application           n'empêchait pas le démarrage

  Domaine en `NXDOMAIN` sur le poste local         Résolution DNS ou cache Comparer avec un résolveur public et
                                                   local pas encore à jour vérifier l'enregistrement DNS

  `404` sur `/`                                    Aucun endpoint racine   Tester une route API connue
                                                   n'est nécessairement    
                                                   défini                  

  `401` sur `/api/products`                        Route protégée et token Obtenir un JWT puis l'envoyer comme
                                                   absent/invalide         Bearer Token

  `L'Issuer JWT est absent de la configuration.`   Variable d'issuer       Ajouter la variable correspondante et
                                                   manquante ou nom de     redémarrer
                                                   configuration incorrect 

  Colonne `"Sku"` introuvable en PostgreSQL        La colonne réelle       Utiliser le nom exact et la casse de la
                                                   s'appelle `"SKU"`       colonne

  Valeur `CreatedAt` absente                       Colonne obligatoire     Inclure `CreatedAt` dans l'insertion, par
                                                   sans valeur par défaut  exemple avec `NOW()`
  ------------------------------------------------------------------------------------------------------------------

## 8. Utilisation de l'API avec Postman

Pour tester l'API déployée :

1.  Créer une requête avec la méthode et l'URL du endpoint.
2.  Pour une route protégée, ouvrir **Authorization**.
3.  Sélectionner **Bearer Token**.
4.  Coller le JWT retourné par la connexion.
5.  Envoyer la requête et vérifier le code HTTP ainsi que le corps JSON.

Exemple de test de lecture des produits :

``` http
GET https://stockpilot-api.yemlahi.org/api/products
Authorization: Bearer <JWT>
```

Le nom exact de la route doit être vérifié dans les contrôleurs ou les
mappings de l'application.

## 9. Git et bonnes pratiques

Avant de pousser le projet sur GitHub :

-   ne pas ajouter les mots de passe, clés JWT, tokens ou chaînes de
    connexion réels ;
-   vérifier `.gitignore` et les fichiers de configuration locaux ;
-   utiliser des variables d'environnement pour les secrets de
    production ;
-   ne jamais ajouter de token de session ou de compte de démonstration
    contenant un mot de passe réel ;
-   garder les scripts de démonstration sans données personnelles ni
    secrets ;
-   lancer `dotnet test` avant un commit.

Exemple de commandes :

``` bash
git status
dotnet test
git add .
git commit -m "docs: document StockPilot architecture, tests and deployment"
git push
```

Si un secret a déjà été commité ou partagé, le remplacer immédiatement.
Supprimer le texte du dernier commit ne suffit pas toujours si le secret
est présent dans l'historique Git.

## 10. Prochaines améliorations

-   Ajouter des tests d'intégration automatisés avec
    `WebApplicationFactory`.
-   Ajouter un environnement de test isolé et une stratégie de nettoyage
    des données.
-   Ajouter un pipeline CI GitHub Actions pour restaurer, compiler et
    exécuter les tests à chaque push ou pull request.
-   Documenter précisément les endpoints, les DTO et les codes de
    réponse à partir du code réel.
-   Ajouter une validation de configuration au démarrage pour signaler
    clairement les variables JWT manquantes.
-   Vérifier les règles de rôle `Admin` et `Employee` sur chaque
    endpoint sensible.
-   Connecter le frontend Angular et gérer le stockage et l'expiration
    des tokens de manière appropriée.
-   Vérifier la configuration HTTPS, les logs de production et la
    stratégie de rotation des secrets.

## 11. Résumé

StockPilot dispose d'un backend ASP.NET Core organisé en couches, de
tests unitaires xUnit validés pour les scénarios principaux du service
produit, d'une base PostgreSQL hébergée sur AlwaysData et d'une API
accessible via un domaine personnalisé. L'authentification JWT a été
configurée et vérifiée avec Postman. Les tests HTTP manuels en
production ont confirmé la protection d'un endpoint et l'accès avec un
token valide.

La prochaine étape technique importante consiste à ajouter et exécuter
des tests d'intégration automatisés, puis à mettre en place une CI pour
compiler le backend et exécuter les tests de façon systématique.
