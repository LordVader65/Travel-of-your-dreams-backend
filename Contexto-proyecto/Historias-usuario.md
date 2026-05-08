# El presente documento tiene los requerimiento que se deben implementar para el sistema de reservas de atracciones turisticas a manera de historias de usuario y sistema.

## Épica 1: Registro, login y autenticación

HU01: Como cliente, quiero registrarme con mis datos personales, correo y contraseña para poder acceder a la plataforma.

HU02: Como cliente, quiero iniciar sesión con mi correo y contraseña para acceder a mis reservas, pagos y facturas (usuario de la aplicacion).

HU03: Como administrador, quiero iniciar sesión con credenciales internas para acceder al panel administrativo.

HU04: Como sistema, quiero validar las credenciales del usuario para permitir o rechazar el acceso.

HU05: Como sistema, quiero diferenciar entre rol cliente y rol administrador para controlar los permisos dentro de la plataforma.

HU06: Como sistema, quiero generar un token de autenticación después del login para proteger las operaciones privadas.

HU07: Como cliente, quiero cerrar sesión para finalizar mi acceso a la plataforma.

HU08: Como administrador, quiero activar o desactivar cuentas de usuarios para controlar el acceso al sistema.

## Épica 2: Gestión del perfil del cliente

HU09: Como cliente, quiero consultar mi información personal para verificar que mis datos estén correctos.

HU10: Como cliente, quiero actualizar mis datos personales, teléfono y dirección para mantener mi perfil actualizado.

HU11: Como cliente, quiero actualizar mi correo electrónico siempre que no esté registrado por otro usuario.

HU12: Como cliente, quiero cambiar mi contraseña para mantener segura mi cuenta.

HU13: Como administrador, quiero consultar la información de los clientes registrados para dar soporte o validar reservas.

HU14: Como administrador, quiero activar o desactivar clientes cuando sea necesario por motivos operativos o de seguridad.

## Épica 3: Consulta de atracciones turísticas

HU15: Como cliente, quiero consultar el listado de atracciones turísticas disponibles para conocer las opciones de reserva.

HU16: Como cliente, quiero filtrar atracciones por destino, categoría, precio, idioma o disponibilidad para encontrar opciones según mis preferencias.

HU17: Como cliente, quiero ver el detalle de una atracción para conocer descripción, ubicación, imágenes, duración, requisitos, inclusiones y restricciones.

HU18: Como cliente, quiero consultar las imágenes de una atracción para tener una mejor referencia visual antes de reservar.

HU19: Como cliente, quiero consultar qué incluye una atracción para saber qué servicios forman parte del paquete.

HU20: Como cliente, quiero consultar los idiomas disponibles de una atracción para elegir una opción adecuada.

HU21: Como cliente, quiero consultar reseñas de otros usuarios para evaluar la calidad de la atracción.

HU22: Como sistema, quiero mostrar solo atracciones activas y disponibles para evitar que el cliente reserve opciones no válidas.

## Épica 4: Gestión administrativa de atracciones

HU23: Como administrador, quiero crear una atracción turística con su información principal para publicarla en la plataforma.

HU24: Como administrador, quiero actualizar los datos de una atracción para corregir información o modificar detalles comerciales.

HU25: Como administrador, quiero activar o desactivar una atracción para controlar si aparece disponible al cliente.

HU26: Como administrador, quiero asociar una atracción a una o varias categorías para mejorar su clasificación.

HU27: Como administrador, quiero asociar una atracción a uno o varios idiomas para indicar en qué idiomas se ofrece.

HU28: Como administrador, quiero asociar imágenes a una atracción para mejorar su presentación visual.

HU29: Como administrador, quiero asociar elementos incluidos a una atracción para informar qué servicios contiene.

HU30: Como administrador, quiero consultar el listado de atracciones con filtros para gestionar el catálogo de forma eficiente.

HU31: Como administrador, quiero eliminar lógicamente una atracción para ocultarla sin perder su historial.

Épica 5: Gestión de destinos, categorías, idiomas, imágenes e inclusiones

HU32: Como administrador, quiero crear destinos turísticos para organizar las atracciones por ubicación.

HU33: Como administrador, quiero actualizar destinos turísticos para mantener correcta la información geográfica.

HU34: Como administrador, quiero activar o desactivar destinos turísticos para controlar su uso en el catálogo.

HU35: Como administrador, quiero crear categorías de atracciones para clasificar mejor la oferta turística.

HU36: Como administrador, quiero actualizar categorías para mantener la clasificación del sistema.

HU37: Como administrador, quiero crear idiomas disponibles para asociarlos a las atracciones.

HU38: Como administrador, quiero crear elementos incluidos, como guía, transporte o alimentación, para asociarlos a las atracciones.

HU39: Como administrador, quiero registrar imágenes para usarlas en las atracciones.

HU40: Como administrador, quiero consultar todos estos catálogos para reutilizarlos al crear o editar atracciones.

## Épica 6: Gestión de tickets y precios

HU41: Como administrador, quiero crear tipos de ticket para una atracción, como adulto, niño, estudiante o tercera edad.

HU42: Como administrador, quiero definir el precio de cada tipo de ticket para que el sistema calcule correctamente las reservas.

HU43: Como administrador, quiero actualizar precios de tickets para reflejar cambios comerciales.

HU44: Como administrador, quiero activar o desactivar tipos de ticket para controlar qué opciones puede seleccionar el cliente.

HU45: Como cliente, quiero ver los tipos de ticket disponibles de una atracción para seleccionar los que necesito.

HU46: Como cliente, quiero ver el precio de cada ticket antes de reservar para conocer el costo total.

HU47: Como sistema, quiero calcular el subtotal por tipo de ticket según cantidad y precio unitario.

## Épica 7: Gestión de horarios y cupos

HU48: Como administrador, quiero crear horarios para una atracción indicando fecha, hora y cupos disponibles.

HU49: Como administrador, quiero actualizar horarios para modificar fecha, hora o capacidad disponible.

HU50: Como administrador, quiero activar o desactivar horarios para controlar cuáles pueden reservarse.

HU51: Como cliente, quiero consultar los horarios disponibles de una atracción para elegir cuándo asistir.

HU52: Como cliente, quiero ver los cupos disponibles antes de reservar para saber si todavía hay disponibilidad.

HU53: Como sistema, quiero impedir reservas en horarios vencidos para evitar inconsistencias.

HU54: Como sistema, quiero impedir reservas cuando no existan cupos suficientes.

HU55: Como sistema, quiero reducir los cupos del horario cuando se crea una reserva válida.

HU56: Como sistema, quiero devolver cupos cuando una reserva pendiente expire o sea cancelada según las reglas del negocio.

HU57: Como sistema, quiero desactivar automáticamente horarios pasados o sin cupos disponibles.

## Épica 8: Creación de reservas

HU58: Como cliente, quiero crear una reserva seleccionando atracción, horario y cantidad de tickets para apartar mi visita.

HU59: Como cliente, quiero ver el resumen de mi reserva antes de confirmarla para validar cantidades, fecha, hora y total.

HU60: Como sistema, quiero validar que el cliente exista y esté activo antes de crear una reserva.

HU61: Como sistema, quiero validar que la atracción, horario y tickets estén activos antes de permitir la reserva.

HU62: Como sistema, quiero validar que existan cupos suficientes antes de confirmar la reserva.

HU63: Como sistema, quiero calcular automáticamente el total de la reserva.

HU64: Como sistema, quiero crear el detalle de la reserva por cada tipo de ticket seleccionado.

HU65: Como sistema, quiero generar un código único de reserva para que el cliente pueda identificarla.

HU66: Como sistema, quiero crear la reserva en estado pendiente hasta que el cliente realice el pago.

HU67: Como sistema, quiero asignar una fecha de expiración a la reserva pendiente para liberar cupos si no se paga a tiempo.

HU68: Como administrador, quiero crear una reserva en nombre de un cliente para dar soporte en canales internos.

## Épica 9: Consulta y gestión de reservas del cliente

HU69: Como cliente, quiero consultar mis reservas para revisar mis compras pendientes, pagadas, canceladas o usadas.

HU70: Como cliente, quiero ver el detalle de una reserva para conocer atracción, horario, tickets, cantidades, total y estado.

HU71: Como cliente, quiero cancelar una reserva pendiente para liberar los cupos si ya no deseo asistir.

HU72: Como cliente, quiero cancelar una reserva pagada si la política del negocio lo permite.

HU73: Como sistema, quiero impedir que el cliente cancele una reserva vencida, usada o no cancelable.

HU74: Como sistema, quiero actualizar el estado de la reserva cuando sea cancelada.

HU75: Como sistema, quiero devolver cupos cuando la cancelación corresponda según las reglas definidas.

HU76: Como cliente, quiero recibir una confirmación del estado final de mi reserva después de cancelar.

## Épica 10: Gestión administrativa de reservas

HU77: Como administrador, quiero consultar todas las reservas del sistema para dar seguimiento operativo.

HU78: Como administrador, quiero filtrar reservas por cliente, fecha, atracción, estado o código para encontrarlas rápidamente.

HU79: Como administrador, quiero ver el detalle completo de una reserva para atender reclamos o validar información.

HU80: Como administrador, quiero cambiar el estado de una reserva cuando el proceso operativo lo requiera.

HU81: Como administrador, quiero marcar una reserva como usada cuando el cliente asista a la atracción.

HU82: Como administrador, quiero marcar una reserva como no show cuando el cliente no asista.

HU83: Como sistema, quiero registrar el historial de cambios de estado de una reserva para trazabilidad.

HU84: Como sistema, quiero aplicar las reglas de devolución o no devolución de cupos según el cambio de estado.

HU85: Como administrador, quiero registrar observaciones sobre una reserva para documentar incidencias.

## Épica 11: Pagos

HU86: Como cliente, quiero pagar una reserva pendiente para confirmar mi compra.

HU87: Como cliente, quiero seleccionar un método de pago disponible para completar la transacción.

HU88: Como sistema, quiero validar que la reserva esté pendiente y no vencida antes de aceptar el pago.

HU89: Como sistema, quiero impedir pagos sobre reservas canceladas, usadas, expiradas o inexistentes.

HU90: Como sistema, quiero registrar el pago con monto, fecha, método y estado.

HU91: Como sistema, quiero cambiar la reserva a estado pagada cuando el pago sea aprobado.

HU92: Como cliente, quiero recibir confirmación del pago aprobado para saber que mi reserva está confirmada.

HU93: Como administrador, quiero consultar pagos realizados para control financiero.

HU94: Como administrador, quiero filtrar pagos por fecha, cliente, reserva, método o estado.

HU95: Como administrador, quiero registrar o validar un pago manual si el negocio permite pagos fuera de línea.

## Épica 12: Datos de facturación

HU96: Como cliente, quiero registrar mis datos de facturación para que se emita correctamente mi factura.

HU97: Como cliente, quiero seleccionar datos de facturación existentes al momento de pagar o facturar.

HU98: Como cliente, quiero actualizar mis datos de facturación para futuras compras.

HU99: Como sistema, quiero validar identificación, razón social, correo y dirección de facturación antes de emitir la factura.

HU100: Como sistema, quiero permitir que un cliente tenga varios datos de facturación.

HU101: Como administrador, quiero consultar los datos de facturación de un cliente para soporte administrativo.

## Épica 13: Facturación

HU102: Como sistema, quiero generar una factura automáticamente después de un pago aprobado.

HU103: Como sistema, quiero asociar la factura a la reserva, pago, cliente y datos de facturación correspondientes.

HU104: Como sistema, quiero calcular subtotal, impuestos y total de la factura.

HU105: Como cliente, quiero consultar mis facturas para revisar mis comprobantes de compra.

HU106: Como cliente, quiero ver el detalle de una factura para conocer los valores cobrados.

HU107: Como administrador, quiero consultar todas las facturas emitidas para control contable.

HU108: Como administrador, quiero filtrar facturas por cliente, fecha, reserva, número o estado.

HU109: Como sistema, quiero impedir generar más de una factura activa para el mismo pago cuando no corresponda.

HU110: Como sistema, quiero mantener trazabilidad de la factura generada.

## Épica 14: Reseñas y calificaciones

HU111: Como cliente, quiero registrar una reseña sobre una atracción después de haber usado una reserva.

HU112: Como cliente, quiero calificar una atracción para compartir mi experiencia con otros usuarios.

HU113: Como sistema, quiero validar que el cliente solo pueda reseñar atracciones que realmente reservó y usó.

HU114: Como sistema, quiero impedir que un cliente registre varias reseñas para la misma reserva si no está permitido.

HU115: Como cliente, quiero consultar las reseñas publicadas de una atracción antes de reservar.

HU116: Como administrador, quiero consultar reseñas para monitorear la calidad del servicio.

HU117: Como administrador, quiero ocultar o desactivar reseñas inapropiadas si incumplen las normas de la plataforma.

## Épica 15: Panel administrativo y reportes básicos

HU118: Como administrador, quiero ver un resumen de reservas por estado para conocer la operación general.

HU119: Como administrador, quiero ver la cantidad de reservas por atracción para identificar las más solicitadas.

HU120: Como administrador, quiero ver ingresos generados por fecha o rango de fechas para control financiero.

HU121: Como administrador, quiero ver horarios con pocos cupos disponibles para anticipar la demanda.

HU122: Como administrador, quiero ver clientes con más reservas para análisis comercial.

HU123: Como administrador, quiero exportar o consultar reportes básicos para seguimiento del negocio.

Épica 16: Auditoría y trazabilidad

HU124: Como sistema, quiero registrar quién creó, modificó o eliminó información importante para mantener trazabilidad.

HU125: Como sistema, quiero registrar cambios críticos en reservas, pagos, facturas, horarios y atracciones.

HU126: Como administrador, quiero consultar eventos de auditoría para revisar acciones realizadas en el sistema.

HU127: Como sistema, quiero guardar fecha, usuario, acción, entidad afectada e información relevante del cambio.

HU128: Como sistema, quiero registrar la IP o servicio origen cuando una operación sea realizada desde la API.

## Épica 17: Seguridad y permisos

HU129: Como sistema, quiero permitir que solo usuarios autenticados accedan a operaciones privadas.

HU130: Como sistema, quiero permitir que el cliente solo consulte y modifique su propia información.

HU131: Como sistema, quiero permitir que el cliente solo consulte sus propias reservas, pagos y facturas.

HU132: Como sistema, quiero permitir que el administrador consulte información global del sistema.

HU133: Como sistema, quiero impedir que un cliente acceda a endpoints administrativos.

HU134: Como sistema, quiero impedir operaciones no autorizadas aunque el usuario intente modificar la petición manualmente.

HU135: Como administrador, quiero gestionar usuarios del sistema para activar, desactivar o cambiar roles cuando sea necesario.

Épica 18: Notificaciones básicas

HU136: Como cliente, quiero recibir confirmación cuando cree una reserva para saber que fue registrada.

HU137: Como cliente, quiero recibir confirmación cuando realice un pago exitoso.

HU138: Como cliente, quiero recibir aviso cuando una reserva esté próxima a vencer.

HU139: Como cliente, quiero recibir confirmación cuando una reserva sea cancelada.

HU140: Como administrador, quiero recibir o consultar alertas sobre reservas vencidas, pagos pendientes o cupos agotados.

## Épica 19: Expiración automática de reservas

HU141: Como sistema, quiero identificar reservas pendientes cuya fecha de expiración ya pasó.

HU142: Como sistema, quiero cambiar automáticamente reservas pendientes vencidas a estado expirada.

HU143: Como sistema, quiero devolver los cupos de reservas expiradas al horario correspondiente.

HU144: Como sistema, quiero impedir que una reserva expirada pueda pagarse.

HU145: Como administrador, quiero consultar reservas expiradas para seguimiento operativo.

