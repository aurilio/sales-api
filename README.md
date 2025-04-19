# 🛒 Sales API

Esta é a implementação que consiste no desenvolvimento de uma API RESTful para gerenciamento de vendas, utilizando arquitetura limpa, boas práticas de engenharia de software e tecnologias modernas como .NET 8, PostgreSQL.

---

## 🚀 Tecnologias e Padrões Utilizados

- **.NET 8** com Minimal APIs
- **PostgreSQL** como banco de dados relacional
- **FluentValidation** para validação dos comandos
- **AutoMapper** para mapeamento de objetos
- **xUnit**, **FluentAssertions**, **Bogus**, **AutoFixture**, **NSubstitute** para testes
- **Serilog** para logging estruturado com suporte à rastreabilidade
- **Docker e Docker Compose** para provisionamento dos serviços
- **Swagger** para documentação interativa
- **SOLID**, **DDD**, **Clean Code**, **GitFlow** e **Semantic Commit** como boas práticas

---


## 🧱 Estrutura do Projeto

```bash
├── src
│   ├── Application       # Handlers e comandos
│   ├── Domain            # Entidades, VO e regras de negócio
│   ├── ORM               # Repositórios e EF Core
│   ├── Messaging         # Publicação de eventos
│   ├── Common            # CrossCutting: Logging, Exceptions, Validations
│   ├── WebApi            # Web API (Controller + Swagger + Middleware)
│   └── IoC               # Injeção de dependência
├── tests
│   └── Unit              # Testes unitários
├── docker-compose.yml                              # Orquestração dos serviços
├── README.md                                        # Este arquivo

```

## ✅ Requisitos

Antes de rodar a aplicação, certifique-se de que os seguintes requisitos estejam instalados na sua máquina:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Docker (2 opções de instalação)

Você pode escolher entre uma das opções abaixo para utilizar o Docker no ambiente Windows:

#### 🔹 Opção 1: Docker Desktop (recomendado)

A maneira mais simples e completa de rodar Docker no Windows é utilizando o Docker Desktop:

- [Download Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Compatível com Windows 10/11 (versões Pro e Home)
- Já inclui integração com WSL 2

> ⚠️ É necessário habilitar o WSL 2 e a Virtualização no BIOS.

#### 🔹 Opção 2: Docker + Docker Compose via WSL (Windows com Ubuntu)

Se preferir não usar o Docker Desktop, você pode instalar diretamente o `docker.io` dentro de uma distribuição Linux (como Ubuntu) via WSL:

1. Instale e configure o [WSL 2](https://learn.microsoft.com/pt-br/windows/wsl/install)
2. Instale o Ubuntu pela Microsoft Store
3. No Ubuntu, execute:

```bash
sudo apt update
sudo apt install docker.io docker-compose-plugin
sudo service docker start
```



## ⚙️ Como executar localmente

### 1. Clone o repositório
```bash
git clone https://github.com/aurilio/sales-api.git
cd sales-api
```

### 2. Suba os serviços com Docker
- Você pode rodar via Visual Studio 2022 ou via terminal com:

```bash
docker-compose up -d
```

### 3.  Execute a aplicação
```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

## 📚 Endpoints disponíveis
- Acesse a documentação Swagger para explorar todos os endpoints e testar as operações disponíveis:
```bash
https://localhost:7181/swagger
```


---

```markdown

## 🧪 Executar os testes

Para rodar os testes unitários:

```bash
cd tests/Ambev.DeveloperEvaluation.Unit
dotnet test

```


---

```markdown
## 📦 Publicação de eventos de domínio

Os seguintes eventos de domínio são publicados (simulados via log):

- `CreatedEvent`
- `ModifiedEvent`
- `CancelledEvent`
- `DeleteEvent`

```
## 📝 Commits e Versionamento

Este projeto segue o padrão **[Semantic Commit Messages](https://www.conventionalcommits.org/en/v1.0.0/)** e o fluxo de trabalho **GitFlow**.

A versão atual é: `v1.0.0`


## 📌 Observações

- A aplicação trata exceções com middleware global (`ValidationExceptionMiddleware`) retornando respostas no formato `ProblemDetails`.
- Está preparada para deploy em nuvem com containers.
- Todo o domínio segue validação explícita e proteção de invariantes.

## 👨‍💻 Autor

**Aurílio Mendes**  
Desenvolvedor .NET Sênior | Arquiteto de Software em formação  
LinkedIn: [@aurilio](https://www.linkedin.com/in/auriliomendes/)


## 📃 Licença

Este projeto é apenas para fins de aprendizado.

