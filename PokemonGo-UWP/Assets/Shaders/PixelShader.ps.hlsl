Texture2D pokemon : register(t0);
SamplerState pokemonSampler : register(s0);

struct PixelShaderInput
{
    float2 tex : TEXCOORD;
};

float4 main(PixelShaderInput input) : SV_TARGET
{
    return pokemon.Sample(pokemonSampler, input.tex);
}