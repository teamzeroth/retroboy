Shader "Hidden Shadow Mask" {

	Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Color2("Color", Color) = (1,1,1,1)
    }
    
    SubShader {
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
 
            fixed4 _Color2;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color2;
                
                return OUT;
            }
 
            sampler2D _MainTex;
            
            half4 frag(v2f IN) : SV_Target {
            	fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                if (c.a < 0.2) discard;
                c.rgb *= c.a;
                c.a = 0;
                return c;
                
                //return IN.color;
            }
            ENDCG
        }
        
        Pass {
        	
        	Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
        	
            Stencil {
                Ref 2
                Comp equal
                Pass keep 
                Fail decrWrap
                ZFail keep
            }
        	
        	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
 
            fixed4 _Color;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                //OUT.color.rgb = 1;
                
                return OUT;
            }
 
            sampler2D _MainTex;
            
            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);
 
                return color;
            }
            
            half4 frag(v2f IN) : SV_Target {
            	//fixed4 c = IN.color;
                //if (c.a < 0.2) discard;
                //c.rgb *= c.a;
                //c.a = 0;
                //return c;
                
                //return half4(0,1,1,1);
                
                //fixed4 c = IN.color; //only color shadow
                
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                if (c.a < 0.2) discard;
                c.rgb *= c.a;
                //c.a = 0;
                
                return c;
            }
            ENDCG
        }
    } 
}