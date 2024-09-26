# Project Name

## Description

This is a backend written for Task manager service. It is written in C# using .NET Core 8. It uses Entity Framework Core for data access and mongodb as the database. The API is RESTful and uses JWT for authentication. The project is containerized using Docker.
This is azure function which is deployed to azue using publish profile. This is auto scalable.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Installation

1. Clone the repository:
    git clone https://github.com/nageshajab/task_manager_service.git
2. Navigate to the project directory:
3. Restore the dependencies:
    dotnet restore

## Usage

1. Build the project:
    dotnet build
1. Run the project:
	dotnet run
1. Open the browser and navigate to:
	https://localhost:5001

    
## Contributing

1. Fork the repository.
2. Create a new branch
  git checkout -b feature_branch_name
3. Make your changes and commit them:
git commit -m "add ur feature"
4. Push to the branch:
    git push origin feature/your-feature
5. Open a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
