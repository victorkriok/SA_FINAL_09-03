# Auth e login API

API REST desenvolvida em **.NET Minimal API** para gerenciamento de usuários e autenticação.
O foco principal do sistema é fornecer **autenticação segura** utilizando **hash de senha com BCrypt** e **tokens JWT**.

Esta API foi projetada com uma estrutura simples e organizada, separando responsabilidades entre **modelos, rotas, DTOs e acesso a dados**.

---

# Arquitetura do Projeto

A aplicação segue uma organização simples baseada em **Minimal APIs**:

```
api
├── model
├── dto
├── routes
├── data
└── services
```

## model

Contém as **entidades do sistema**, que representam as tabelas do banco de dados.

Exemplos:

* `Usuario`
* `Perfil`

Essas classes são usadas pelo **Entity Framework Core** para mapear os dados no PostgreSQL.

---

## dto

Os **DTOs (Data Transfer Objects)** representam os dados que entram ou saem da API.

Eles existem para:

* evitar exposição direta dos modelos
* validar entradas
* controlar exatamente o que o cliente pode enviar

Exemplos:

* `UsuarioDTO`
* `LoginDTO`
* `PerfilDTO`

---

## routes

Contém as **rotas da API** utilizando o padrão de **Minimal API do ASP.NET**.

Cada arquivo organiza um conjunto de endpoints relacionados.

Exemplos:

* `UsuarioRoutes`
* `PerfilRoutes`

---

## data

Contém o **DbContext**, responsável pela conexão com o banco de dados PostgreSQL através do **Entity Framework Core**.

Classe principal:

```
ApiContext
```

Ela define os `DbSet` que representam as tabelas do banco.

---

# Segurança da API

A API utiliza duas camadas principais de segurança:

## Hash de senha com BCrypt

As senhas **nunca são armazenadas em texto puro no banco de dados**.

Durante o cadastro de usuário:

1. a senha enviada pelo cliente é processada pelo **BCrypt**
2. o BCrypt gera automaticamente **hash + salt**
3. apenas o hash resultante é salvo no banco

Exemplo de hash gerado:

```
$2a$11$7FzV0S2h6rXhZp8D3nGqYOb5qgG7YQjF2wF6WQ3u1F2Xx7Zk9Hk2K
```

O salt já está incluído dentro do hash gerado pelo BCrypt.

Durante o login, o BCrypt compara a senha enviada com o hash armazenado.

---

## Autenticação com JWT

Após validar o login, a API gera um **JSON Web Token (JWT)**.

O token contém informações do usuário autenticado, como:

* identificador do usuário
* login
* perfil

Esse token é enviado ao cliente e deve ser utilizado nas requisições seguintes.

Exemplo de uso:

```
Authorization: Bearer TOKEN_AQUI
```

A API valida o token antes de permitir acesso a endpoints protegidos.

---

# Funcionalidades da API

A API possui três funcionalidades principais:

1. **Criação de perfis**
2. **Criação de usuários**
3. **Autenticação de usuários (login)**

A funcionalidade mais importante do sistema é o **login**, pois é ele que fornece o **token de acesso (JWT)**.

Os endpoints de criação de **perfil** e **usuário** existem principalmente para permitir que o sistema possua contas válidas para autenticação.

---

# Modelos do Sistema

## Perfil

Representa o nível de acesso de um usuário.

Campos principais:

* Id
* Nome
* Descricao

Perfis serão utilizados futuramente para controle de permissões no sistema.

---

## Usuario

Representa um usuário do sistema.

Campos principais:

* Id
* Nome
* Login
* SenhaHash
* PerfilId
* Ativo
* CriadoEm

A senha do usuário é armazenada apenas como **hash gerado pelo BCrypt**.

---

# Rotas da API

## Criar Perfil

Cria um novo perfil de acesso no sistema.

Endpoint:

```
POST /perfis
```

Body esperado:

```
{
  "nome": "ADMIN",
  "descricao": "Acesso total ao sistema"
}
```

Resposta:

```
{
  "id": "uuid",
  "nome": "ADMIN",
  "descricao": "Acesso total ao sistema"
}
```

---

## Criar Usuário

Cria um novo usuário no sistema.

A senha enviada é automaticamente convertida em **hash BCrypt** antes de ser salva no banco.

Endpoint:

```
POST /usuarios
```

Body esperado:

```
{
  "nome": "Bernardo",
  "login": "bernardo",
  "senha": "123456",
  "perfilId": "uuid-do-perfil"
}
```

---

## Login (Endpoint Principal)

Este é o endpoint mais importante da API.

Ele valida as credenciais do usuário e retorna um **token JWT** que permitirá acessar endpoints protegidos.

Endpoint:

```
POST /login
```

Body esperado:

```
{
  "login": "bernardo",
  "senha": "123456"
}
```

Resposta:

```
{
  "message": "User liberado",
  "token": "JWT_TOKEN"
}
```

Se as credenciais estiverem incorretas, a resposta indicará falha na autenticação.

---

# Fluxo de Autenticação

Fluxo típico de uso da API:

1. Criar um perfil
2. Criar um usuário associado ao perfil
3. Realizar login
4. Receber um token JWT
5. Utilizar o token nas requisições autenticadas

Fluxo simplificado:

```
Criar Perfil
    ↓
Criar Usuário
    ↓
Login
    ↓
Receber JWT
    ↓
Acessar endpoints protegidos
```

---

# Tecnologias Utilizadas

* ASP.NET Core Minimal API
* Entity Framework Core
* PostgreSQL
* BCrypt.Net
* JWT Authentication
* Swagger (OpenAPI)

---

# Observações

* As senhas são protegidas utilizando **BCrypt com salt automático**.
* O sistema utiliza **JWT para autenticação stateless**.
* A arquitetura foi mantida simples utilizando **Minimal API** para facilitar manutenção e leitura do código.
* O controle de acesso baseado em **perfil** poderá ser expandido futuramente para autorização por roles.
