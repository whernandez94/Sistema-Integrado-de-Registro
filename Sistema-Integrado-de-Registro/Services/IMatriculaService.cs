﻿using Sistema_Integrado_de_Registro.DTO;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IMatriculaService
    {
        Task<List<MatriculaDetailsDto>> GetAllMatriculasAsync();
        Task<MatriculaEditDto?> GetMatriculaByIdAsync(int id);
        Task<ServiceResult> SaveMatriculaAsync(Matricula matricula);
        Task<ServiceResult> DeleteMatriculaAsync(int id);
        Task<List<Estudiante>> GetEstudiantesDisponiblesAsync();
        Task<List<Seccion>> GetSeccionesDisponiblesAsync();
        Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync();
        Task<string> GenerarNumeroExpedienteAsync();
    }


}
