namespace AtraccionesTuristicas.Backend.LA.Business.DTOs.Catalogo;

public sealed class AtraccionDetalleResponse : AtraccionPublicaResponse
{
    public IReadOnlyList<CategoriaResponse> Categorias { get; set; } = [];
    public IReadOnlyList<IdiomaResponse> Idiomas { get; set; } = [];
    public IReadOnlyList<ImagenResponse> Imagenes { get; set; } = [];
    public IReadOnlyList<IncluyeResponse> Incluye { get; set; } = [];
    public IReadOnlyList<TicketResponse> Tickets { get; set; } = [];
    public IReadOnlyList<ReseniaResponse> Resenias { get; set; } = [];
}

