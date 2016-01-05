//#include "UnityCG.cginc"

float waveOffset( float _p, float _Waves, float _Times, float _Ignore ){
    float p = abs( _p );
    float cosV = cos( p * _Waves - _Times );
    // Ignoreの距離分だけスロープを作る
    float reduct = clamp ( p / _Ignore, 0, 1 );
    return cosV * reduct;
}

float4 vert_wave( float4 vertex, float _Waves, float _Times, float _Ignore, float3 _Fact )
{
    float4 vPos = vertex;
    vPos.w = vertex.w;

    vPos.x = vertex.x + waveOffset( vertex.y, _Waves, _Times, _Ignore ) * _Fact.x;
    vPos.y = vertex.y + waveOffset( vertex.z, _Waves, _Times, _Ignore ) * _Fact.y;
    vPos.z = vertex.z + waveOffset( vertex.x, _Waves, _Times, _Ignore ) * _Fact.z;
    return vPos;
}

