using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Geotrend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstrumentoController : ControllerBase
{
    private readonly IInstrumentoService _instrumentoService;

    public InstrumentoController(IInstrumentoService instrumentoService)
    {
        _instrumentoService = instrumentoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarInstrumentoInputDto dto)
    {
        try
        {
            var resultado = await _instrumentoService.CriarAsync(dto);
            return CreatedAtAction(nameof(Criar), new { id = resultado.Id }, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}