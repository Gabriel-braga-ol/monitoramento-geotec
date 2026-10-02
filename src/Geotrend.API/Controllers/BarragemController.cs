using Geotrend.Application.DTOs;
using Geotrend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Geotrend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarragemController : ControllerBase
{
    private readonly IBarragemService _barragemService;

    public BarragemController(IBarragemService barragemService)
    {
        _barragemService = barragemService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarBarragemInputDto dto)
    {
        try
        {
            var resultado = await _barragemService.CriarBarragemAsync(dto);
            return CreatedAtAction(nameof(Criar), new { id = resultado.Id }, resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var barragens = await _barragemService.ObterTodasAsync();
        return Ok(barragens);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var resultado = await _barragemService.ObterPorIdAsync(id);
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> ObterStatusResumo(Guid id)
    {
        var resultado = await _barragemService.ObterStatusResumoAsync(id);
        if (resultado == null)
            return NotFound(new { mensagem = "Barragem não encontrada." });

        return Ok(resultado);
    }
}