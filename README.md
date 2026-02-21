# Agenda de contatos - v2

## Sobre o projeto 

O intuito desse projeto, foi aplicar outros aprendizados que tive, decorrente a estudos e cursos que realizei desde o ultimo projeto.

Claro que a melhoria não foi somente na parte de back end, mas também na parte de front end, trazendo uma visualização e usabilidade melhor, utilizando todos os novos conhecimentos que adquirir.

Nesta versão, o foco foi aprofundar conceitos de:

- Arquitetura MVC
- Autenticação com JWT
- Modelagem avançada com Fluent API
- Organização em camadas (Domain, Data, Services)
- Melhorias de UI/UX no front-end

O projeto representa um salto técnico em relação à versão anterior, tanto na estrutura do back-end quanto na organização e experiência do front-end.

## Objetivo

Desenvolver um aplicativo Web, utilizando C#, .NET, ASP.NET CORE, com razor pages no padrão de arquitetura MVC (model - view - controller) tendo tudo integrado, e colocando em prática os novos conhecimentos que foram, ASP.NET CORE e Razor pages
algo no qual, até o momento não tive contato.

A aplicação contém as seguintes funcionalidades:
- Criar contatos
- Criar grupos
- Editar contatos e grupos
- Excluir contatos e grupos
- Relacionar contatos a grupos
- Cadastro e login de usuários
- Autenticação via JWT Token
- Proteção de rotas autenticadas

---
## Conhecimentos aplicados

- Autenticação com JWT
- Entity Framework Core + Fluent API
- Organização em Camadas

## Tecnologias Utilizadas

### Back-end
- C# com .NET 9
- ASP.NET CORE
- SQL Server (persistência de dados)
- Entity Framework core
- JWT (Json Web Token)
- Fluent API

### Front-end
- Razor pages
- Bootstrap 5
- Javascript puro (Fetch API)

## 📸 Preview da Aplicação

<p align="center">
  <img src="docs/home.png" width="700"/>
</p>

## Como rodar o projeto

git clone https://github.com/guiilhermex/AgendaDeContato2.git
- abra o arquivo appsettings.json
- altere a connectionString
- no terminal dotnet ef database update
- dotnet run

