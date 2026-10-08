# RNF17 - Interoperabilidad con Stripe

## Objetivo
Validar que el servicio de pagos con Stripe mantiene un contrato estable, interpreta correctamente los eventos del proveedor y maneja errores de forma consistente.

## Evidencia revisada
- Creación de PaymentIntent
- Consulta de estado del pago
- Respuesta de error del SDK
- Webhook de `payment_intent.succeeded`
- Validación cuando la configuración de Stripe no existe

## Resultado
La interoperabilidad quedó validada con pruebas automatizadas que cubren contrato, formato y manejo de errores del servicio de pagos.
