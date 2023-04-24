using System;
using System.Runtime.CompilerServices;
using ILGPU;

namespace BAVCL.Experimental
{
	public partial class TestCls
	{
		#region "CBRT"

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public unsafe static float CbrtFast(double N, bool checksafety = true)
		//{

		//    float n = (float)N;

		//    // Perform "Safety Check" for; -n, 0, +Inf, -Inf, NaN
		//    if (checksafety)
		//    {
		//        if (n < 0) { return -CbrtFast(-n); }
		//        if (n == 0 || double.IsNaN(n) || double.IsInfinity(n)) { return n; }
		//    }

		//    // Initial approximation
		//    // Convert the binary representation of float into a positive int
		//    // Isolate & Convert mantissa into actual power it represents
		//    // Perform cube root on 2^P, to appoximate x using power law
		//    float x = 1 << (int)((((*(uint*)&N) >> 53) - 1023) * 0.33333333333333333333f) + 0b1;

		//    // Perform check if x^3 matches n
		//    float xcubed = x * x * x;
		//    if (xcubed == n) { return x; }

		//    // Perform 4 itterations of Halley algorithm for double accuracy
		//    float xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    return x;
		//}

		//public static float CbrtFastInterop(double N, bool checksafety = true)
		//{

		//    float n = (float)N;

		//    // Perform "Safety Check" for; -n, 0, +Inf, -Inf, NaN
		//    if (checksafety)
		//    {
		//        if (n < 0) { return -CbrtFastInterop(-n); }
		//        if (n == 0 || double.IsNaN(n) || double.IsInfinity(n)) { return n; }
		//    }

		//    // Initial approximation
		//    // Convert the binary representation of float into a positive int
		//    // Isolate & Convert mantissa into actual power it represents
		//    // Perform cube root on 2^P, to appoximate x using power law

		//    float x = 1 << ((int)(((Interop.FloatAsInt(N) >> 53) - 1023) * 0.33333333333333333333f) + 0b1);

		//    // Perform check if x^3 matches n
		//    float xcubed = x * x * x;
		//    if (xcubed == n) { return x; }

		//    // Perform 4 itterations of Halley algorithm for double accuracy
		//    float xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    return x;
		//}

		//public static float CbrtFastInterop2(double N, bool checksafety = true)
		//{

		//    float n = (float)N;

		//    // Perform "Safety Check" for; -n, 0, +Inf, -Inf, NaN
		//    if (checksafety)
		//    {
		//        if (n < 0) { return -CbrtFastInterop2(-n); }
		//        if (n == 0 || double.IsNaN(n) || double.IsInfinity(n)) { return n; }
		//    }

		//    // Initial approximation
		//    // Convert the binary representation of float into a positive int
		//    // Isolate & Convert mantissa into actual power it represents
		//    // Perform cube root on 2^P, to appoximate x using power law

		//    float x = 1 << (int)(((Interop.FloatAsInt(N) >> 53) - 1023) * 0.33333333333333333333f) + 0b1;

		//    // Perform check if x^3 matches n
		//    float xcubed = x * x * x;
		//    if (xcubed == n) { return x; }

		//    // Perform 4 itterations of Halley algorithm for double accuracy
		//    float xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) / (xcubed + xcubedPlusN));

		//    return x;
		//}

		//public unsafe static float CbrtFastRcp(double N, bool checksafety = true)
		//{

		//    float n = (float)N;

		//    // Perform "Safety Check" for; -n, 0, +Inf, -Inf, NaN
		//    if (checksafety)
		//    {
		//        if (n < 0) { return -CbrtFastRcp(-n); }
		//        if (n == 0 || double.IsNaN(n) || double.IsInfinity(n)) { return n; }
		//    }

		//    // Initial approximation
		//    // Convert the binary representation of float into a positive int
		//    // Isolate & Convert mantissa into actual power it represents
		//    // Perform cube root on 2^P, to appoximate x using power law
		//    float x = 1 << (int)((((*(uint*)&N) >> 53) - 1023) * 0.33333333333333333333f) + 0b1;

		//    // Perform check if x^3 matches n
		//    float xcubed = x * x * x;
		//    if (xcubed == n) { return x; }

		//    // Perform 4 itterations of Halley algorithm for double accuracy
		//    float xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = x * x * x;
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    return x;
		//}

		//public unsafe static float CbrtFastRcpANDPow(double N, bool checksafety = true)
		//{

		//    float n = (float)N;

		//    // Perform "Safety Check" for; -n, 0, +Inf, -Inf, NaN
		//    if (checksafety)
		//    {
		//        if (n < 0) { return -CbrtFastRcpANDPow(-n); }
		//        if (n == 0 || double.IsNaN(n) || double.IsInfinity(n)) { return n; }
		//    }

		//    // Initial approximation
		//    // Convert the binary representation of float into a positive int
		//    // Isolate & Convert mantissa into actual power it represents
		//    // Perform cube root on 2^P, to appoximate x using power law
		//    float x = 1 << (int)((((*(uint*)&N) >> 53) - 1023) * 0.33333333333333333333f) + 0b1;

		//    // Perform check if x^3 matches n

		//    float xcubed = XMath.Pow(x, 3);
		//    if (xcubed == n) { return x; }

		//    // Perform 4 itterations of Halley algorithm for double accuracy
		//    float xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = XMath.Pow(x, 3);
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = XMath.Pow(x, 3);
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = XMath.Pow(x, 3);
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = XMath.Pow(x, 3);
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    xcubed = XMath.Pow(x, 3);
		//    xcubedPlusN = xcubed + n;
		//    x *= ((xcubedPlusN + n) * XMath.Rcp(xcubed + xcubedPlusN));

		//    return x;
		//}

		#endregion

		#region "SQRT"

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sqrt(float N)
		{
			// if N < 0 throw error, no support for imaginary numbers
			int n = (int)Interop.FloatAsInt(N);

			int M = (n >> 23) - 127;
			float result = 1 << (M >> 1);

			if ((M & 0b1) == 0b1)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;
			
			if (D == 0) { return result; }

			float d = Interop.IntAsFloat((uint)(0b0_01111111_00000000000000000000000 | D));

			result *= (0.0239878f * d * d * d - 0.17811632f * d * d + 0.78060805f * d + 0.37368985f);

			return result;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sqrt_simplif_v5(float N)
		{
			int n = (int)Interop.FloatAsInt(N);

			float result = 1 << ((n >> 23) - 127 >> 1);

			if ((~n & 0b100000000000000000000000) == 0b100000000000000000000000)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;

			if (D == 0) { return result; }

			float d = Interop.IntAsFloat(0b0_01111111_00000000000000000000000 | D);

			result *= d * (d * (d * ((0.00492272f * d - 0.0471846f) * d + 0.194967f) - 0.474473f) + 1.02803f) + 0.293744f;

			return result;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sqrt_taylor_v2(float N)
		{
			int n = (int)Interop.FloatAsInt(N);

			float result = 1 << ((n >> 23) - 127 >> 1);

			if ((~n & 0b100000000000000000000000) == 0b100000000000000000000000)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;

			if (D == 0) { return result; }

			float d = Interop.IntAsFloat(0b0_01111111_00000000000000000000000 | D);

			result = result * 0.00390625f * (d * (d * (d * (d * (7f * d - 45f) + 126f) - 210f) + 315) + 63);

			return result;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sqrt_acc_v1(float N)
		{
			int n = (int)Interop.FloatAsInt(N);

			float result = 1 << ((n >> 23) - 127 >> 1);

			if ((~n & 0b100000000000000000000000) == 0b100000000000000000000000)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;

			if (D == 0) { return result; }


			float d = Interop.IntAsFloat(0b0_01111111_00000000000000000000000 | D);

			// 1.5 <= d < 2 
			if ((D & 0b0_00000000_10000000000000000000000) == 0b0_00000000_10000000000000000000000)
			{
				result *= (0.41166212f + (0.71114645f * d) - (0.13609814f * d * d) + (0.01558198f * d * d * d));
				return 0.5f * (result + (N/result));
			}

			// 1 <= d < 1.5
			result *= (0.3464898f + (0.84429014f * d) - (0.22725081f * d * d) + (0.03648872f * d * d * d));
			return 0.5f * (result + (N / result));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sqrt_acc_v2(float N)
		{
			int n = (int)Interop.FloatAsInt(N);

			float result = 1 << ((n >> 23) - 127 >> 1);

			if ((~n & 0b100000000000000000000000) == 0b100000000000000000000000)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;

			if (D == 0) { return result; }


			float d = Interop.IntAsFloat(0b0_01111111_00000000000000000000000 | D);

			// 1.5 <= d < 2 
			if ((D & 0b0_00000000_10000000000000000000000) == 0b0_00000000_10000000000000000000000)
			{
				result *= (d * ((0.015582f * d - 0.136098f) * d + 0.711146f) + 0.411662f);
				return 0.5f * (result + (N / result));
			}

			// 1 <= d < 1.5
			result *= (d * ((0.0364887f * d - 0.227251f) * d + 0.84429f) + 0.34649f);
			return 0.5f * (result + (N / result));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sqrt_acc_v3(float N)
		{
			int n = (int)Interop.FloatAsInt(N);

			float result = 1 << ((n >> 23) - 127 >> 1);

			if ((~n & 0b100000000000000000000000) == 0b100000000000000000000000)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;

			if (D == 0) { return result; }


			float d = Interop.IntAsFloat(0b0_01111111_00000000000000000000000 | D);

			// 1.5 <= d < 2 
			if ((D & 0b0_00000000_10000000000000000000000) == 0b0_00000000_10000000000000000000000)
			{
				result *= (d * ((0.015582f * d - 0.136098f) * d + 0.711146f) + 0.411662f);
				return 0.5f * (result + (N / result));
			}

			// 1 <= d < 1.5
			result *= (d * ((0.0364887f * d - 0.227251f) * d + 0.84429f) + 0.34649f);
			return 0.5f * (result + (N / result));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float sqrt_acc_vcpu(float N)
		{
			int n = *(int*)&N;

			float result = 1 << ((n >> 23) - 127 >> 1);

			if ((~n & 0b100000000000000000000000) == 0b100000000000000000000000)
			{
				result *= 1.414213562373095f;
			}

			uint D = (uint)(n << 9) >> 9;

			if (D == 0) { return result; }

			float d = Interop.IntAsFloat(0b0_01111111_00000000000000000000000 | D);

			// 1.5 <= d < 2 
			if ((D & 0b0_00000000_10000000000000000000000) == 0b0_00000000_10000000000000000000000)
			{
				result *= (d * ((0.015582f * d - 0.136098f) * d + 0.711146f) + 0.411662f);
				return 0.5f * (result + (N / result));
			}

			// 1 <= d < 1.5
			result *= (d * ((0.0364887f * d - 0.227251f) * d + 0.84429f) + 0.34649f);
			return 0.5f * (result + (N / result));
		}

		#endregion


		#region SQRT v2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float newSQRT(float x)
		{
			int b = *(int*)&x;
			float result = 1 << ((b >> 23) - 63);
			if ((~b & 0b100000000000000000000000) == 0b100000000000000000000000)
				result *= 1.414213562373095f;

			b &= 0b0_00000000_01111111111111111111111;

			if (b == 0) return result;

			// puts 1 before the decimal point
			b |= 0b0_01111111_00000000000000000000000;
			float d = *(float*)&b;

			// 1.5 <= d < 2 
			if ((b & 0b0_00000000_10000000000000000000000) == 0b0_00000000_10000000000000000000000)
			{
				result *= (d * ((0.015582f * d - 0.136098f) * d + 0.711146f) + 0.411662f);
				return 0.5f * (result + (x / result));
			}

			// 1 <= d < 1.5
			result *= (d * ((0.0364887f * d - 0.227251f) * d + 0.84429f) + 0.34649f);
			return 0.5f * (result + (x / result));
		}
		
		
		#endregion


		// DOUBLE OUTPUT

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static double CBRT(double N)
		{
			if (N < 0) { return -CBRT(-N); }
			if (N == 0 || double.IsInfinity(N) || double.IsNaN(N))
				return N;

			ulong n = Interop.FloatAsInt(N);
			//ulong n = *(ulong*)&N;

			ulong M = 
				(n & 0b0_00000000000_1111111111111111111111111111111111111111111111111111) 
				   | 0b0_01111111111_0000000000000000000000000000000000000000000000000000;
			int E = (int)((((n >> 52) - 1023) << 2) / 3);

			double result = 1 << (E >> 2);

			// 2^(1/3)
			if ((E & 0b1) == 0b1)
				result *= 1.25992104989487316476721060727822835057025f;
			// 2^(2/3)
			else if ((E & 0b10) == 0b10)
				result *= 1.587401051968199474751705639272308260391493f;

			//double d = *(double*)&M;
			double d = Interop.IntAsFloat(M);

			// 1.5 <= d < 2 
			if (d > 1.5f)
				result *= (0.595096f + d * (0.510083f + (-0.11659f + 0.0138798f * d) * d));
			else
				result *= (0.531962f + d * (0.638351f + (-0.204272f + 0.0340454f * d) * d));

			double resultcubed = result * result * result;
			double resultcubedPlusN = resultcubed + N;
			result *= ((resultcubedPlusN + N) / (resultcubed + resultcubedPlusN));
			double resultSquared = result * result;
			result -= ((resultSquared * result - N) / 
				(resultSquared + resultSquared + resultSquared));

			return result;
		}



		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public unsafe static double Cbrt(float N)
		//{
		//    // Cover edge cases
		//    if (N < 0)
		//        return -Cbrt(-N);
		//    if (N == 0 || float.IsInfinity(N) || float.IsNaN(N))
		//        return N;

		//    // Initial approximation
		//    uint n = Interop.FloatAsInt(N);
		//    uint M =
		//        (n & 0b0_00000000_11111111111111111111111)
		//        | 0b0_01111111_00000000000000000000000;
		//    int E = (int)((((n >> 23) - 127) << 2) / 3);
		//    double result = 1 << (E >> 2);

		//    // Second Approximation
		//    // Case 2^(1/3)
		//    if ((E & 0b1) == 0b1)
		//        result *= 1.25992104989487316476721060727822835057025f;
		//    // Case 2^(2/3)
		//    else if ((E & 0b10) == 0b10)
		//        result *= 1.587401051968199474751705639272308260391493f;

		//    // Third Approximation
		//    double d = Interop.IntAsFloat(M);
		//    if (d > 1.5f)
		//        result *= (0.595096f + d * (0.510083f + (-0.11659f + 0.0138798f * d) * d));
		//    else
		//        result *= (0.531962f + d * (0.638351f + (-0.204272f + 0.0340454f * d) * d));

		//    double resultcubed = result * result * result;
		//    double resultcubedPlusN = resultcubed + N;
		//    result *= ((resultcubedPlusN + N) / (resultcubed + resultcubedPlusN));
		//    double resultSquared = result * result;
		//    result -= ((resultSquared * result - N) /
		//        (resultSquared + resultSquared + resultSquared));

		//    return result;
		//}


		// FLOAT 32 OUTPUT
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float CbrtF(float N)
		{
			// Cover edge cases
			if (N < 0)
				return -CbrtF(-N);
			if (N == 0 || float.IsInfinity(N) || float.IsNaN(N))
				return N;

			// Initial approximation
			// Convert the binary representation of float into a positive int
			// Isolate mantissa & Convert exponent into actual power it represents
			// Perform cube root on 2^P, to appoximate x using power law
			uint n = Interop.FloatAsInt(N);
			uint M =
				(n & 0b0_00000000_11111111111111111111111)
				| 0b0_01111111_00000000000000000000000;
			int E = (int)((((n >> 23) - 127) << 2) / 3);
			float result = 1 << (E >> 2);

			// Second Approximation
			// Recovers lost precision from dividing power by 3
			// Case 2^(1/3)
			if ((E & 0b1) == 0b1)
				result *= 1.25992104989487316476721060727822835057025f;
			// Case 2^(2/3)
			else if ((E & 0b10) == 0b10)
				result *= 1.587401051968199474751705639272308260391493f;

			// Third Approximation
			// Uses the mantissa to improve accuracy
			float d = Interop.IntAsFloat(M);
			if (d > 1.5f)
				// Taylor expansion @ 1.75 covers range of 1.5 <= d < 2
				result *= (0.595096f + d * (0.510083f + (-0.11659f + 0.0138798f * d) * d));
			else
				// Taylor expansion @ 1.25 covers range of 1 <= d < 1.5
				result *= (0.531962f + d * (0.638351f + (-0.204272f + 0.0340454f * d) * d));

			// Perform Newton + Halley itteration
			float resultcubed = result * result * result;
			float resultcubedPlusN = resultcubed + N;
			result *= ((resultcubedPlusN + N) / (resultcubed + resultcubedPlusN));
			float resultSquared = result * result;
			result -= ((resultSquared * result - N) /
				(resultSquared + resultSquared + resultSquared));

			return result;
		}
		
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static float CbrtF(double N)
		{
			// Cover edge cases
			if (N < 0)
				return -CbrtF(-N);
			if (N == 0 || double.IsInfinity(N) || double.IsNaN(N))
				return (float)N;

			// Initial approximation
			// Convert the binary representation of float into a positive int
			// Isolate mantissa & Convert exponent into actual power it represents
			// Perform cube root on 2^P, to appoximate x using power law
			ulong n = Interop.FloatAsInt(N);
			ulong M =
				(n & 0b0_00000000_11111111111111111111111)
				| 0b0_01111111_00000000000000000000000;
			int E = (int)((((n >> 23) - 127) << 2) / 3);
			double result = 1 << (E >> 2);

			// Second Approximation
			// Recovers lost precision from dividing power by 3
			// Case 2^(1/3)
			if ((E & 0b1) == 0b1)
				result *= 1.25992104989487316476721060727822835057025f;
			// Case 2^(2/3)
			else if ((E & 0b10) == 0b10)
				result *= 1.587401051968199474751705639272308260391493f;

			// Third Approximation
			// Uses the mantissa to improve accuracy
			double d = Interop.IntAsFloat(M);
			if (d > 1.5f)
				// Taylor expansion @ 1.75 covers range of 1.5 <= d < 2
				result *= (0.595096f + d * (0.510083f + (-0.11659f + 0.0138798f * d) * d));
			else
				// Taylor expansion @ 1.25 covers range of 1 <= d < 1.5
				result *= (0.531962f + d * (0.638351f + (-0.204272f + 0.0340454f * d) * d));

			// Perform Newton + Halley itteration
			double resultcubed = result * result * result;
			double resultcubedPlusN = resultcubed + N;
			result *= ((resultcubedPlusN + N) / (resultcubed + resultcubedPlusN));
			double resultSquared = result * result;
			result -= ((resultSquared * result - N) /
				(resultSquared + resultSquared + resultSquared));

			return (float)result;
		}          // <-- This function needs type optimisation



		//This is a port from Rust https://github.com/rust-lang/libm/blob/master/src/math/cbrt.rs
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double CbrtRust(double x)
		{
			const int b1 = 715094163; /* B1 = (1023-1023/3-0.03306235651)*2**20 */
			const int b2 = 696219795; /* B2 = (1023-1023/3-54/3-0.03306235651)*2**20 */

			const double p0 = 1.87595182427177009643; /* 0x3ffe03e6, 0x0f61e692 */
			const double p1 = -1.88497979543377169875; /* 0xbffe28e0, 0x92f02420 */
			const double p2 = 1.621429720105354466140; /* 0x3ff9f160, 0x4a49d6c2 */
			const double p3 = -0.758397934778766047437; /* 0xbfe844cb, 0xbee751d9 */
			const double p4 = 0.145996192886612446982; /* 0x3fc2b000, 0xd4e4edd7 */

			double x1p54 = Interop.IntAsFloat(0x4350000000000000); // 0x1p54 === 2 ^ 54
			ulong ui = Interop.FloatAsInt(x);
			double r;
			double s;
			double t;
			double w;
			uint hx = ((uint)(ui >> 32)) & 0x7fffffff;

			if (hx >= 0x7ff00000)
			{
				/* cbrt(NaN,INF) is itself */
				return x + x;
			}

			/*
			 * Rough cbrt to 5 bits:
			 *    cbrt(2**e*(1+m) ~= 2**(e/3)*(1+(e%3+m)/3)
			 * where e is integral and >= 0, m is real and in [0, 1), and "/" and
			 * "%" are integer division and modulus with rounding towards minus
			 * infinity.  The RHS is always >= the LHS and has a maximum relative
			 * error of about 1 in 16.  Adding a bias of -0.03306235651 to the
			 * (e%3+m)/3 term reduces the error to about 1 in 32. With the IEEE
			 * floating point representation, for finite positive normal values,
			 * ordinary integer divison of the value in bits magically gives
			 * almost exactly the RHS of the above provided we first subtract the
			 * exponent bias (1023 for doubles) and later add it back.  We do the
			 * subtraction virtually to keep e >= 0 so that ordinary integer
			 * division rounds towards minus infinity; this is also efficient.
			 */
			if (hx < 0x00100000)
			{
				/* zero or subnormal? */
				ui = Interop.FloatAsInt(x * x1p54);
				hx = (uint)(ui >> 32) & 0x7fffffff;
				if (hx == 0) return x; /* cbrt(0) is itself */
				hx = hx / 3 + b2;
			}
			else
			{
				hx = hx / 3 + b1;
			}
			ui &= (ulong)1 << 63;
			ui |= (ulong)hx << 32;
			t = Interop.IntAsFloat(ui);

			/*
			 * New cbrt to 23 bits:
			 *    cbrt(x) = t*cbrt(x/t**3) ~= t*P(t**3/x)
			 * where P(r) is a polynomial of degree 4 that approximates 1/cbrt(r)
			 * to within 2**-23.5 when |r - 1| < 1/10.  The rough approximation
			 * has produced t such than |t/cbrt(x) - 1| ~< 1/32, and cubing this
			 * gives us bounds for r = t**3/x.
			 *
			 * Try to optimize for parallel evaluation as in __tanf.c.
			 */
			r = (t * t) * (t / x);
			t = t * ((p0 + r * (p1 + r * p2)) + ((r * r) * r) * (p3 + r * p4));

			/*
			 * Round t away from zero to 23 bits (sloppily except for ensuring that
			 * the result is larger in magnitude than cbrt(x) but not much more than
			 * 2 23-bit ulps larger).  With rounding towards zero, the error bound
			 * would be ~5/6 instead of ~4/6.  With a maximum error of 2 23-bit ulps
			 * in the rounded t, the infinite-precision error in the Newton
			 * approximation barely affects third digit in the final error
			 * 0.667; the error in the rounded t can be up to about 3 23-bit ulps
			 * before the final error is larger than 0.667 ulps.
			 */
			ui = Interop.FloatAsInt(t);
			ui = (ui + 0x80000000) & 0xffffffffc0000000;
			t = Interop.IntAsFloat(ui);

			/* one step Newton iteration to 53 bits with error < 0.667 ulps */
			s = t * t; /* t*t is exact */
			r = x / s; /* error <= 0.5 ulps; |r| < |t| */
			w = t + t; /* t+t is exact */
			r = (r - t) / (w + r); /* r-t is exact; w+r ~= 3*t */
			t = t + t * r; /* error <= 0.5 + 0.5/3 + epsilon */
			return t;
		}


		#region "Logs"

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static double LOG2(double N)
		{
			ulong n = *(ulong*)&N;
			int E = (int)((n >> 52) - 1023);       
			n &= 0b0_00000000000_1111111111111111111111111111111111111111111111111111;
			n |= 0b0_01111111111_0000000000000000000000000000000000000000000000000000;
			double d = *(double*)&n;
			d -= 1.0f;
			return (d * (d * (d * ((0.0379969f * d - 0.166236f) * d + 0.379969f) - 0.688693f) + 1.43676f) + 0.000463584f) + E;         
		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static double LOG2_v2(double N)
		{
			ulong n = *(ulong*)&N;
			int E = (int)((n >> 52) - 1023);
			n &= 0b0_00000000000_1111111111111111111111111111111111111111111111111111;
			n |= 0b0_01111111111_0000000000000000000000000000000000000000000000000000;
			double d = *(double*)&n;
			// range of d is 1 <= d < 2
			// @d = 1 => return 0
			// @d = 2 => return 1
			// result will be between 0 <= r < 1 

			ulong x = n & 0b0_00000000000_1100000000000000000000000000000000000000000000000000;

			switch (x)
			{
				// range 1.75 <= x < 2.0
				case 0b0_00000000000_1100000000000000000000000000000000000000000000000000:
					return (d * (d * (d * ((0.0124508f * d - 0.145908f) * d + 0.72954f) - 2.05183f) + 3.84718f) -2.38726f) + E;

				// range 1.50 <= x < 1.75
				case 0b0_00000000000_1000000000000000000000000000000000000000000000000000:
					return (d * (d * (d * ((0.0254646f * d - 0.258625f) * d + 1.12071f) - 2.73173f) + 4.43906f) - 2.59371f) + E;

				// range 1.25 <= x < 1.50
				case 0b0_00000000000_0100000000000000000000000000000000000000000000000000:
					return (d * (d * (d * ((0.0587072f * d - 0.504515f) * d + 1.84989f) - 3.81539f) + 5.24616f) - 2.83472f) + E;

				// range 1.00 <= x < 1.25
				case 0b0_00000000000_0000000000000000000000000000000000000000000000000000:
					return (d * (d * (d * ((0.160119f * d - 1.12584f) * d + 3.37751f) - 5.69954f) + 6.41199f) - 3.12423f) + E;
			}

			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static double LOG10(double N)
		{
			ulong n = *(ulong*)&N;
			int E = (int)((n >> 52) - 1023);
			n &= 0b0_00000000000_1111111111111111111111111111111111111111111111111111;
			n |= 0b0_01111111111_0000000000000000000000000000000000000000000000000000;
			double d = *(double*)&n;
			d -= 1.0f;
			return ((d * (d * (d * ((0.0379969f * d - 0.166236f) * d + 0.379969f) - 0.688693f) + 1.43676f) + 0.000463584f) + E) * 0.3010299956640140308365204351253505410896f;
		}

		#endregion


	}
}
