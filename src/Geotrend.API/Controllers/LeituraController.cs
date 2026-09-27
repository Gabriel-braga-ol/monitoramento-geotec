using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Geotrend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeituraController : ControllerBase
{
    private readonly ILeituraService _leituraService;

    public LeituraController(ILeituraService leituraService)
    {
        _leituraService = leituraService;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarLeituraInputDto dto)
    {
        try
        {
            var resultado = await _leituraService.RegistrarLeituraAsync(dto);
            return CreatedAtAction(nameof(Registrar), new { id = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro interno ao processar leitura.", detalhe = ex.Message });
        }
    }
}