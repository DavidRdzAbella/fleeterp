using FleetErp.Infrastructure.Identity;
using FluentAssertions;

namespace FleetErp.UnitTests.Infrastructure;

/// <summary>
/// La llave de firma se deriva con SHA-256 en lugar de usar los bytes literales
/// del texto configurado. Estas pruebas fijan ese comportamiento porque de él
/// depende que una frase corta no tumbe la emisión de tokens en producción.
/// </summary>
public class JwtOptionsTests
{
    [Theory]
    [InlineData("D7avid-Richi3e-FleetErp.Api")]                       // 27 caracteres
    [InlineData("corta")]                                             // 5 caracteres
    [InlineData("una-frase-larga-de-mas-de-treinta-y-dos-caracteres")] // 49 caracteres
    public void Cualquier_longitud_produce_una_llave_de_256_bits(string secret)
    {
        var key = new JwtOptions { SigningKey = secret }.CreateSigningKey();

        key.KeySize.Should().Be(256);
    }

    [Fact]
    public void La_derivacion_es_estable_para_que_quien_firma_y_quien_valida_coincidan()
    {
        var options = new JwtOptions { SigningKey = "D7avid-Richi3e-FleetErp.Api" };

        var primera = options.CreateSigningKey().Key;
        var segunda = options.CreateSigningKey().Key;

        segunda.Should().Equal(primera);
    }

    [Fact]
    public void Dos_secretos_distintos_producen_llaves_distintas()
    {
        var a = new JwtOptions { SigningKey = "secreto-uno" }.CreateSigningKey().Key;
        var b = new JwtOptions { SigningKey = "secreto-dos" }.CreateSigningKey().Key;

        b.Should().NotEqual(a);
    }

    [Fact]
    public void Sin_secreto_configurado_la_aplicacion_no_debe_arrancar()
    {
        var act = () => new JwtOptions { SigningKey = "   " }.CreateSigningKey();

        act.Should().Throw<InvalidOperationException>().WithMessage("*Jwt:SigningKey*");
    }
}
