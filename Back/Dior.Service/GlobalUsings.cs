// Global usings pour le projet Dior.Service
global using Dior.Library.DTO;
global using Dior.Library.DTO.User;
global using Dior.Library.DTO.Access;
global using Dior.Library.DTO.Role;
global using Dior.Data.DTO.Team;
global using Dior.Library.DTO.Audit;
global using Dior.Library.DTO.Task;
global using Dior.Library.DTO.Contract;
global using Dior.Library.DTO.Payroll;
global using Dior.Library.DTO.Message;
global using Dior.Library.DTO.Notification;
global using Dior.Library.DTO.Project;
global using Microsoft.Extensions.Logging;
global using Dior.Service.Services;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

// Compatibility aliases
global using UpdateProjetRequest = Dior.Library.DTO.Project.UpdateProjetDto;
global using CreateProjetRequest = Dior.Library.DTO.Project.CreateProjetDto;
global using UpdateTaskRequest = Dior.Library.DTO.Task.UpdateTaskDto;
global using CreateTaskRequest = Dior.Library.DTO.Task.CreateTaskDto;
global using UpdateContractRequest = Dior.Library.DTO.Contract.UpdateContractDto;
global using CreateContractRequest = Dior.Library.DTO.Contract.CreateContractDto;