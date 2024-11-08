# Todo App

A simple and efficient Todo application built with .NET Core for the API and a React frontend powered by Vite. This app leverages Docker for streamlined setup and deployment, following clean architecture and component design principles.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Features](#features)
3. [Tech Stack](#tech-stack)
4. [Prerequisites](#prerequisites)
5. [Installation](#installation)
   - [Clone the Repository](#clone-the-repository)
   - [Environment Setup](#environment-setup)
   - [Build Docker Containers](#build-docker-containers)
6. [Usage](#usage)
7. [Configuration](#configuration)
8. [Testing](#testing)
   - [API Tests](#api-tests)

## Project Overview

This Todo App helps users efficiently manage tasks. It uses Docker to streamline development and deployment, with a simple and intuitive UI.

**[Live Demo](http://ec2-18-232-92-36.compute-1.amazonaws.com/)**

## Features

- Full CRUD (Create, Read, Update, Delete) operations for managing todos
- Dockerized backend and frontend services for easy setup
- Clean and user-friendly interface for task management
- Styled with Tailwind CSS for responsive and modern design
- Unit tests for the backend with xUnit

## Tech Stack

- **Backend**: .NET Core, SQLite
- **Frontend**: React, Vite, Tailwind CSS
- **Containerization**: Docker
- **Cloud**: AWS (ECR, EC2, CloudFormation)
- **Testing**: xUnit

## Prerequisites

- **Docker**: Ensure Docker is installed. [Get Docker](https://docs.docker.com/get-docker/)
- **Node.js** (if running frontend without Docker): [Download Node.js](https://nodejs.org/)

## Installation

1. **Clone the Repository**:  
   `git clone https://github.com/allansalgadocr/todo-app.git`  
   `cd todo-app`

2. **Environment Setup**:

   - Create a `.env` file in the `api` directory to configure necessary environment variables (e.g., policy name, allowed origin, api key).
   - Ensure sensitive environment variables are kept secure and are not exposed in production environments.

3. **Build Docker Containers**:  
   In the root directory, run the following command to build and start the backend and frontend services:  
   `docker-compose up --build`

   This will set up the following services:

   - **API**: Accessible on `localhost:3001`
   - **Frontend**: Accessible on `localhost:3000`

## Usage

After the containers are running, you can access the application through:

- **Frontend**: Visit `http://localhost:3000` to interact with the Todo App’s user interface.
- **API**: Access the API directly at `http://localhost:3001` for REST endpoints, useful for integrations and testing.

The frontend provides a user-friendly interface for managing todos, while the backend API supports CRUD operations for todos.

## Configuration

- **Docker Configuration**: Review and modify any Docker-related configurations in `docker-compose.yml` as needed.
- **Ports**:
  - **API**: Port `3001`
  - **Frontend**: Port `3000`

## Testing

1. **API Tests**:
   - API tests are located in the `tests` directory.
   - To run these tests with xUnit, navigate to the API project directory:
     `cd api/TodoApp.Api`
     `dotnet test`
