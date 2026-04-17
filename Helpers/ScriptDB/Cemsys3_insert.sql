-- INSERT para CantidadCuotas (según el ejemplo proporcionado)
INSERT INTO CantidadCuotas (cuota) VALUES 
(1),
(2),
(3),
(4),
(5),
(6);

-- INSERT para TiposConceptoTarifaria
INSERT INTO [TemasTarifaria] (nombre) VALUES 
('General'),
('Inhumación'),
('Concesión nicho'),
('Concesión fosa'),
('Registro Civil'),
('Derecho de Oficina'),
('Fondo');

-- INSERT para RolesUsuarios
INSERT INTO RolesUsuarios (rol) VALUES 
('Empleado'),
('Administrador');

-- INSERT para TipoNumeracionParcelas
INSERT INTO TipoNumeracionParcelas (tipoNumeracion) VALUES 
('Nueva (nichos repetidos)'),
('Antigua (sin repetir)');

-- INSERT para TipoNichos
INSERT INTO TipoNichos (tipo) VALUES 
('Féretro'),
('Urnario'),
('Especial');

-- INSERT para AniosConcesion (años típicos de concesión)
INSERT INTO AnioConcesion (anios) VALUES 
(1),
(5),
(10),
(15),
(25);

-- INSERT para CategoriaPersonas
INSERT INTO CategoriasPersonas (categoria) VALUES 
('Titular'),
('Fallecido');

INSERT INTO TipoNota (Descripcion, visibilidad) VALUES 
('Ingreso', 1),
('Recordatorio', 1);

-- INSERT para EstadoDifunto
INSERT INTO EstadosDifunto (estado) VALUES 
('Cuerpo completo'),
('Reducido'),
('Cremado');

-- INSERT para TipoTramite
INSERT INTO TipoTramite (tipo) VALUES 
('Ingreso'), --1
('Autorización para cremación'), --2
('Autorización para reducción'), --3
('Contrato de concesión'), --4
('Autorización para traslado'), --5
('Cambio de titularidad'), --6
('Nota'); --7

INSERT INTO RequisitosTramite (tipoTramiteId, descripcion)
VALUES 
(1, 'Debe presentarse el {titular} con DNI'),
(2, 'Adjuntar acta de defunción si el titular está fallecido'),
(3, 'Presentar libreta de familia'),
(4, 'Adjuntar acta de defunción si el titular está fallecido'),
(5, 'Adjuntar acta de defunción si el titular está fallecido'),
(6, 'Adjuntar acta de defunción si el titular está fallecido');


-- INSERT para TipoParcela
INSERT INTO TipoParcela (tipo) VALUES 
('Nicho'),
('Fosa'),
('Panteón');

-- INSERT para TipoPanteon
INSERT INTO TipoPanteon (tipo) VALUES 
('Con nichos'),
('Sin nichos');

Insert into TipoAutorizacion (tipoTramiteId, nombre) values (6, 'Cambio titular - ambos presentes');

-- INSERT para conceptos
INSERT INTO ConceptosTarifaria (nombre, temaId) VALUES
('Apertura de nicho con placa', 1), --1
('Apertura de nicho sin placa', 1), --2
('Apertura de fosa', 1), --3
('Cierre de nicho', 2), --4
('Cierre de fosa', 2), --5
('Permiso para colocar placa', 1), --6
('Permiso de refacciones', 1), --7
('Reducción', 1),--8
('Cremación', 1),--9
('Inhumación nicho', 2),--10
('Inhumación fosa', 2),--11
('Inhumación panteón', 2),--12
('Defunción', 5),--13
('Transcripción de acta', 5),--14
('Introducción', 6),--15
('Concesión Nicho', 3),--16
('Concesión Fosa', 4),--17
('% de fondo de ayuda centro de salud', 7),--18
('Monto mínimo de fondo', 7),--19
('% de aumento de inhumación de otras localidades',7), --20
('% de aumento de concesiones de otras localidades',7), --21
('% de aumento de introducción derecho de oficina de otras localidades',7), --22
('% de precios de nichos urnarios de secc 16-18',7), --23
('% de introducción de urna de derecho de oficina',7),--24
('% de descuento de renovacion de concesión al dia',7); --25


--INSERT PreciosTarifaria
INSERT INTO PreciosTarifarias (precio, conceptoTarifariaId, visibilidad) values 
(0.00, 1, 1),
(0.00, 2, 1),
(0.00, 3, 1),
(0.00, 4, 1),
(0.00, 5, 1),
(0.00, 6, 1),
(0.00, 7, 1),
(0.00, 8, 1),
(0.00, 9, 1),
(0.00, 10, 1),
(0.00, 11, 1),
(0.00, 12, 1),
(0.00, 13, 1),
(0.00, 14, 1),
(0.00, 15, 1),
(0.05, 18, 1), --fondo %
(1000.00, 19, 1), --monto minimo fondo
(1.00, 20, 1), --% de aumento de inhumación de otras localidades
(0.50, 21, 1), --% de aumento de concesiones de otras localidades
(1.00, 22, 1), --% de aumento de introduccion derecho de oficina de otras localidades
(0.50, 23, 1), --% de precios de nichos urnarios de secc 16-18
(0.25, 24, 1), --% de introduccion de urna de derecho de oficina
(0.30, 25, 1); --% de descuento de renovacion de concesión al dia

--para fosas
INSERT INTO PreciosTarifarias (precio, conceptoTarifariaId, visibilidad, nroFila, aniosConcesionId) values 
(0.00, 17, 1, 1, 4), --15 años
(0.00, 17, 1, 1, 5); --25 años


INSERT INTO EstadosTramites (estado, tipoTramiteId)
VALUES 
('Registrado', 1), --ingreso 1
('Finalizado', 1), --ingreso 2
('Pendiente', 7), --nota 3
('Finalizado', 7), --nota 4
('SinContrato', 4), --contrato de concesion 5
('Vigente', 4), --contrato de concesion 6
('Vencido', 4), --contrato de concesion 7
('Caducado', 4), --contrato de concesion 8
('Iniciado', 6), --cambio de titularidad 9
('Finalizado', 6), --cambio de titularidad 10
('Cancelado', 6); --cambio de titularidad 11

--insert into EstadosTramites (estado, tipoTramiteId) values ('Iniciado', 6), ('Finalizado', 6);
INSERT INTO Usuarios (nombre, apellido, correo, usuario, clave, rolId) values ('Tomas', 'Carreras', 'tomaselle2@gmail.com', 'Tomaselle2', 'P7eSe/VyhW8UaKMx5qghSw==.JYrOO0ZJQLp0A82FUreiYz7mWl+BpZykU1AfM1ZOpZU=', 2);

--precios de nichos
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Fallecidos de otra localidad con domicilio en otra localidad',
1, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Fallecidos de otra localidad con domicilio en Tirolesa',
1, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Fallecidos en Colonia Tirolesa',
1, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Cenizas en urnas con domicilio en Tirolesa',
1, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Nicho - Cenizas en urnas con domicilio en otra localidad',
1, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
1, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
10, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

--precios de fosas
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Fallecidos de otra localidad con domicilio en otra localidad',
2, --tipoParcelaId--fosa
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
5, --cierre fosa --si aplica
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Fallecidos de otra localidad con domicilio en Tirolesa',
2, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
5, --cierre fosa --si aplica
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Fallecidos en Colonia Tirolesa',
2, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
5, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Cenizas en urnas con domicilio en Tirolesa',
2, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Fosa - Cenizas en urnas con domicilio en otra localidad',
2, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
NULL, --tipoPanteonId
11, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

--precios panteon con nichos
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Fallecidos de otra localidad con domicilio en otra localidad',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId --con nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Fallecidos de otra localidad con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId --con nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Fallecidos en Colonia Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Cenizas en urnas con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón con nichos - Cenizas en urnas con domicilio en otra localidad',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
1, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
4, --cierre nicho --si aplica
null, --cierre fosa
1);-- visibilidad

--precios panteon sin nichos
INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Fallecidos de otra localidad con domicilio en otra localidad',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId --sin nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Fallecidos de otra localidad con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId --sin nicho
12, --conceptoInhumacionId
13,--conceptoDefuncionId
14,--conceptoTranscripcionId --si aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Fallecidos en Colonia Tirolesa',
3, --tipoParcelaId--nicho
1, --estadoDifuntoId --cuerpo completo o reduccion
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
null, --porcentajeIntroduccion-Urna-DerechoOficna --no aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Cenizas en urnas con domicilio en Tirolesa',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
null, --porcentajeAumentoInhumacionOtraLocalidadId --no aplica
null, --porcentajeAumentoDerechoOficinaId --no aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

INSERT INTO ReglasIngreso (nombreRegla, tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId, 
conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId, conceptoIntroduccionId, 
porcentajeFondoSaludId, porcentajeAumentoOtraLocalidadId, porcentajeAumentoDerechoOficinaId, 
porcentajeIntroduccionUrnaDerechoOficna, montoMinimoFondoId, cierreNicho, cierreFosa, visibilidad) VALUES
('Panteón sin nichos - Cenizas en urnas con domicilio en otra localidad',
3, --tipoParcelaId--nicho
3, --estadoDifuntoId --cremado
null, --tipoNichoId --feretro o especial
2, --tipoPanteonId
12, --conceptoInhumacionId
13,--conceptoDefuncionId
null,--conceptoTranscripcionId --no aplica
15, --conceptoIntroduccionId
18, --porcentajeFondoSaludId
20, --porcentajeAumentoInhumacionOtraLocalidadId --si aplica
22, --porcentajeAumentoDerechoOficinaId --si aplica
24, --porcentajeIntroduccion-Urna-DerechoOficna --si aplica
19,--montoMinimoFondoId
null, --cierre nicho --no aplica
null, --cierre fosa
1);-- visibilidad

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(6, 'Cambio Titular - Ambos Presentes',
'<p> </p><figure class="image"><img style="aspect-ratio:600/140;" src="/fotos/logoMuni.png" alt="Logo" width="600" height="140"></figure><p> </p><p> </p><p>Colonia Tirolesa, {Fecha}</p><p> </p><p> </p><p><br>Se deja constancia de que {articuloTitularActual} {sr/sraTitularActual} <strong>{TitularesActuales} DNI {DniTitularActual}, </strong>quien suscribiera el contrato de concesión real de uso sobre <strong>{Parcela} </strong>el cual está ocupado por <strong>{Difuntos},</strong> sito en el cementerio Municipal de Colonia Tirolesa cede {articuloNuevoTitular} {sr/sraNuevoTitular} <strong>{NuevosTitulares} DNI {DniNuevosTitulares}. </strong>La concesión del bien mencionado supra de común entre las partes presentes.<br> </p><p> </p><p> </p><p> </p><p> </p><p> </p><p> </p><p> </p><p> </p><p>FIRMA______________________________________________                 DNI___________________________________</p><p> </p><p>FIRMA______________________________________________                 DNI___________________________________</p><p> </p><p> </p><p> </p><p> </p><figure class="image"><img style="aspect-ratio:1024/150;" src="/fotos/pieContrato.png" width="1024" height="150"></figure><p><br> </p><p> </p>',
1, 1); 



--Job para pasar de concesion vigente a vencida 
--BEGIN TRANSACTION;

---- 1. Obtener trámites que pasan a vencido
--DECLARE @TramitesVencidos TABLE (
--    tramiteId INT
--);

--INSERT INTO @TramitesVencidos (tramiteId)
--SELECT t.id
--FROM Tramites t
--INNER JOIN Concesiones c ON c.tramiteId = t.id
--WHERE 
--    c.vencimiento < GETDATE()
--    AND t.estadoActualId = 6; -- Vigente


---- 2. Actualizar estado actual
--UPDATE t
--SET estadoActualId = 7 -- Vencido
--FROM Tramites t
--INNER JOIN @TramitesVencidos tv ON tv.tramiteId = t.id;


---- 3. Insertar historial
--INSERT INTO HistorialEstadoTramite (fecha, tramiteId, estadoTramiteId)
--SELECT 
--    GETDATE(),
--    tramiteId,
--    7 -- Vencido
--FROM @TramitesVencidos;


--COMMIT;
