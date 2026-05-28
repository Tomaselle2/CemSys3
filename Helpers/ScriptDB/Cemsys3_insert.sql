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
('Cremación'), --2
('Reducción'), --3
('Contrato de concesión'), --4
('Traslado'), --5
('Cambio de titularidad'), --6
('Nota'), --7
('Aceptación de titularidad'), --8
('Permiso de ingreso'); --9


-- INSERT para TipoParcela
INSERT INTO TipoParcela (tipo) VALUES 
('Nicho'),
('Fosa'),
('Panteón');

-- INSERT para TipoPanteon
INSERT INTO TipoPanteon (tipo) VALUES 
('Con nichos'),
('Sin nichos');

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
('Iniciado', 6),
('Finalizado', 6), 
('Cancelado', 6),
('Pendiente', 6);

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


Insert into TipoAutorizacion (tipoTramiteId, nombre) values 
(6, 'Cambio titular - ambos presentes'), --1
(8, 'Aceptación de titularidad'),  --2
(2, 'Cremación - Autorización'), --3
(2, 'Cremación - Libre Tránsito'), --4
(2, 'Nuevo Destino - Registro Civil'), --5
(5, 'Traslado - Autorización'), --6
(3, 'Reducción - Autorización'), --7
(9, 'Permiso de ingreso'); --8

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(6, 'Cambio Titular - Ambos Presentes',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: justify;"> </p>  <p style="text-align: justify;">Se deja constancia de que {articuloTitularActual} {sr/sraTitularActual} <strong>{TitularesActuales} DNI {DniTitularActual}, </strong>quien suscribiera el contrato de concesión real de uso sobre <strong>{Parcela} </strong>el cual está ocupado por <strong>{Difuntos},</strong> sito en el cementerio Municipal de Colonia Tirolesa cede {articuloNuevoTitular} {sr/sraNuevoTitular} <strong>{NuevosTitulares} DNI {DniNuevosTitulares}. </strong>La concesión del bien mencionado supra de común entre las partes presentes.<br> </p>  <br><br><br><br><br><br><br><br>  <p class="firma-linea">FIRMA_____________________________________                 DNI___________________________________</p>  <br><br><br>  <p class="firma-linea">FIRMA_____________________________________                 DNI___________________________________</p>  <br><br><br><br><br><br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
1, 1); 

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(8, 'Aceptación de titularidad',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: justify;"> </p>  <p style="text-align: justify;">Se deja constancia de que {articuloNuevoTitular} {sr/sraNuevoTitular} <strong>{NuevosTitulares} DNI {DniNuevosTitulares},</strong><strong> </strong>toma posesión de la titularidad del siguiente <strong>{Parcela} </strong>el cual está ocupado por <strong>{Difuntos} </strong>de este cementerio municipal,<strong> </strong>a causa del fallecimiento del titular anterior. <br> </p>  <br><br><br><br><br><br><br><br>  <p class="firma-linea">FIRMA_____________________________________                </p>  <br><br><br>  <p class="firma-linea">DNI________________________________________</p>  </div>  <p><br><br><br></p>  <div class="documento-contenido">TELÉFONO________________________________________<br><br><br><br><br><br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
2, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(2, 'Cremación - Autorización',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <span style="text-decoration: underline;">S                           //                          D</span><br>  <p style="text-align: justify;">Atentamente:<br>El que suscribe la presente <strong>{NombreCompletoFirmante} DNI {DniFirmante}</strong> en carácter de <strong>{Parentesco}</strong> del difunto <strong>{Difuntos}</strong> inhumado en <strong>{Parcela}</strong> <strong>({NroConcesion}),</strong> de este Cementerio Municipal; autoriza al Municipio a realizar el siguiente trámite: <strong>{AperturaNicho/Fosa}</strong> <strong>Y TRASLADO AL</strong> <strong>{crematorio} </strong>siendo el que suscribe responsable de los posibles daños que se pudieran ocasionar, fruto del trabajo realizado, (féretro, lápida, placa, etc.).<br>     Así mismo y a tales efectos declaro bajo juramento que estoy facultado para el presente requerimiento atento al vinculo expresado en la presente y que soy el único pariente legitimado, o cuento con el consentimiento de ellos. Consecuentemente haciéndome responsable exclusivo de todos los efectos que pudieran derivar de la presente.<br>A los fines de avalar los extremos indicados adjunto los siguientes comprobantes que obran en mi poder y que consisten en recibos pago mantenimiento. Como así También recibos de pago de los aranceles estipulados para dicho trámite por la ordenanza Municipal.<br>     Por cualquier eventualidad relacionada con el presente trámite fijo domicilio en <strong>{DomicilioFirmante}</strong>.</p>  <br><br>  <p class="firma-linea">FIRMA_____________________________________</p>  <br><br>  <p class="firma-linea">ACLARACIÓN_____________________________________</p>  <br><br>  <p class="firma-linea">TELÉFONO_____________________________________</p>  <br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
3, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(2, 'Cremación - Libre Tránsito',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p style="text-align: center;">LIBRE TRÁNSITO - PERMISO DE TRASLADO</p>  <p style="text-align: justify;">Se autoriza al portador de la presente a trasladar los restos mortales del extinto, <strong>{Difuntos}</strong> fallecido el ________________, ubicado en <strong>{Parcela}</strong> desde el cementerio Municipal de Colonia Tirolesa hacia el {crematorioDestino} de esta misma localidad.</p>  <br><br><br>  <p class="firma-linea"> </p>  <br><br>  <p class="firma-linea"> </p>  <br><br>  <p class="firma-linea"> </p>  <br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
4, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(2, 'Nuevo Destino - Registro Civil',
'<div class="documento-contenido">  <figure class="image-logo"><img src="../fotos/EncabezadoRegistro.jpg?v=@DateTime.Now.Ticks" alt="Logo"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <p class="MsoNormal" style="text-align: center;" align="center"><span style="font-size: 12.0pt; line-height: 115%;">SOLICITUD – DEFUNCIONES – DESTINO DE LOS RESTOS</span></p>  <p class="MsoNormal"><span style="font-size: 12.0pt; line-height: 115%;">Al Registro Civil de Colonia Tirolesa</span></p>  <p class="MsoNormal"><span style="font-size: 12.0pt; line-height: 115%;">De mi mayor consideración</span></p>  <p class="MsoNormal" style="text-align: justify; text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;">{NombreCompletoFirmante} D.N.I {DniFirmante}, Domicilio {DomicilioFirmante} en carácter de {Parentesco}, del extinto/a {Difuntos} fallecido el _______________ manifiesto en calidad de declaración jurada contar con interés legítimo, y, para solicitar a Ud.:</span></p>  <p class="MsoNormal" style="text-align: justify; text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;">Deje constancia en el acta de defunción/transcripción de {Difuntos} Registrada bajo el acta ____ Tomo __ Folio ___ Año _____ Serie -<span style="mso-spacerun: yes;">  </span><span style="mso-spacerun: yes;"> </span>localidad de COLONIA TIROLESA, DPTO COLÓN, CÓRDOBA, del traslado de los restos del causante al nuevo destino, siendo éste, {crematorio}.</span></p>  <p class="MsoNormal" style="text-align: justify; text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;">Asimismo declaro que no hay objeción alguna a este trámite por ningún otro familiar que pudiera tener el mismo derecho a disposición que quien suscribe.</span></p>  <p class="MsoNormal" style="text-align: justify; text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;">Se releva de responsabilidad al Registro Civil de esta ciudad, en caso de que existan otras normas testamentarias o de otro tipo sobre la disposición de los restos del causante, incompatibles con la presente y que sea o no de conocimiento del solicitante.</span></p>  <p class="MsoNormal" style="text-align: justify; text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;">Se adjunta a la presente DNI del solicitante, acta de defunción, permiso de salida del cementerio local, documentación que acredita el interés legítimo.</span></p>  <p class="MsoNormal" style="text-align: justify; text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;">Sin otro particular, le saluda atte.-</span></p>  <br><br>  <p class="MsoNormal" style="text-indent: 127.6pt;"><span style="font-size: 12.0pt; line-height: 115%;"> </span></p>  <p class="MsoNormal" style="text-align: right; text-indent: 127.6pt;" align="right"><span style="font-size: 12.0pt; line-height: 115%;">Firma del solicitante</span></p>  <p class="MsoNormal" style="text-align: justify;"><span style="font-size: 12.0pt; line-height: 115%;">Certifico que la firma que antecede ha sido puesta en mi presencia y pertenece a {NombreCompletoFirmante} D.N.I {DniFirmante}</span></p>  <br><br><br><br><br><br><br><br><br><br><br>  <figure class="image-pie"><img src="../fotos/pieRegistro.jpg?v=@DateTime.Now.Ticks"></figure>  </div>',
5, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(5, 'Traslado - Autorización',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <span style="text-decoration: underline;">S                           //                          D</span><br>  <p style="text-align: justify;">Atentamente:<br>El que suscribe la presente <strong>{NombreCompletoFirmante} DNI {DniFirmante}</strong> en carácter de <strong>{Parentesco}</strong> del difunto <strong>{Difuntos}</strong> inhumado en <strong>{Parcela}</strong> <strong>({NroConcesion}),</strong> de este Cementerio Municipal; autoriza al Municipio a realizar el siguiente trámite: <strong>{AperturaNicho/Fosa}</strong> <strong>Y TRASLADO A</strong> <strong>{NuevaUbicacionTraslado} </strong>siendo el que suscribe responsable de los posibles daños que se pudieran ocasionar, fruto del trabajo realizado, (féretro, lápida, placa, etc.).<br>     Así mismo y a tales efectos declaro bajo juramento que estoy facultado para el presente requerimiento atento al vinculo expresado en la presente y que soy el único pariente legitimado, o cuento con el consentimiento de ellos. Consecuentemente haciéndome responsable exclusivo de todos los efectos que pudieran derivar de la presente.<br>A los fines de avalar los extremos indicados adjunto los siguientes comprobantes que obran en mi poder y que consisten en recibos pago mantenimiento. Como así También recibos de pago de los aranceles estipulados para dicho trámite por la ordenanza Municipal.<br>     Por cualquier eventualidad relacionada con el presente trámite fijo domicilio en <strong>{DomicilioFirmante}</strong>.</p>  <br><br>  <p class="firma-linea">FIRMA_____________________________________</p>  <br><br>  <p class="firma-linea">ACLARACIÓN_____________________________________</p>  <br><br>  <p class="firma-linea">TELÉFONO_____________________________________</p>  <br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
6, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(3, 'Reducción - Autorización',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <span style="text-decoration: underline;">S                           //                          D</span><br>  <p style="text-align: justify;">Atentamente:<br>El que suscribe la presente <strong>{NombreCompletoFirmante} DNI {DniFirmante}</strong> en carácter de <strong>{Parentesco}</strong> del difunto <strong>{Difuntos}</strong> inhumado en <strong>{Parcela}</strong> <strong>({NroConcesion}),</strong> de este Cementerio Municipal; autoriza al Municipio a realizar el siguiente trámite: <strong>{AperturaNicho/Fosa}, REDUCCIÓN </strong><strong>{NuevaUbicacionTraslado} </strong>siendo el que suscribe responsable de los posibles daños que se pudieran ocasionar, fruto del trabajo realizado, (féretro, lápida, placa, etc.).<br>     Así mismo y a tales efectos declaro bajo juramento que estoy facultado para el presente requerimiento atento al vinculo expresado en la presente y que soy el único pariente legitimado, o cuento con el consentimiento de ellos. Consecuentemente haciéndome responsable exclusivo de todos los efectos que pudieran derivar de la presente.<br>A los fines de avalar los extremos indicados adjunto los siguientes comprobantes que obran en mi poder y que consisten en recibos pago mantenimiento. Como así También recibos de pago de los aranceles estipulados para dicho trámite por la ordenanza Municipal.<br>     Por cualquier eventualidad relacionada con el presente trámite fijo domicilio en <strong>{DomicilioFirmante}</strong>.</p>  <br><br>  <p class="firma-linea">FIRMA_____________________________________</p>  <br><br>  <p class="firma-linea">ACLARACIÓN_____________________________________</p>  <br><br>  <p class="firma-linea">TELÉFONO_____________________________________</p>  <br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
7, 1);

insert into PlantillasTramite (tipoTramiteId, nombre, contenido, tipoAutorizacionId, activo) values 
(9, 'Permiso de ingreso',
'<div class="documento-contenido">  <figure class="image-logo"><img style="aspect-ratio: 600/140;" src="../fotos/logoMuni.png?v=@DateTime.Now.Ticks" alt="Logo" width="310" height="90"></figure>  <p style="text-align: right;">Colonia Tirolesa, {Fecha}</p>  <span style="text-decoration: underline;">S                           //                          D</span><br>  <p style="text-align: justify;">Atentamente:<br>El que suscribe la presente <strong>{NombreCompletoFirmante} DNI {DniFirmante}</strong> en carácter de <strong>{Parentesco}</strong> del difunto <strong>{Difuntos}</strong> inhumado en <strong>{Parcela}</strong> <strong>({NroConcesion}),</strong> de este Cementerio Municipal; autoriza al Municipio a realizar el siguiente trámite: <strong>{AperturaNicho/Fosa}, REDUCCIÓN </strong><strong>{NuevaUbicacionTraslado} </strong>siendo el que suscribe responsable de los posibles daños que se pudieran ocasionar, fruto del trabajo realizado, (féretro, lápida, placa, etc.).<br>     Así mismo y a tales efectos declaro bajo juramento que estoy facultado para el presente requerimiento atento al vinculo expresado en la presente y que soy el único pariente legitimado, o cuento con el consentimiento de ellos. Consecuentemente haciéndome responsable exclusivo de todos los efectos que pudieran derivar de la presente.<br>A los fines de avalar los extremos indicados adjunto los siguientes comprobantes que obran en mi poder y que consisten en recibos pago mantenimiento. Como así También recibos de pago de los aranceles estipulados para dicho trámite por la ordenanza Municipal.<br>     Por cualquier eventualidad relacionada con el presente trámite fijo domicilio en <strong>{DomicilioFirmante}</strong>.</p>  <br><br>  <p class="firma-linea">FIRMA_____________________________________</p>  <br><br>  <p class="firma-linea">ACLARACIÓN_____________________________________</p>  <br><br>  <p class="firma-linea">TELÉFONO_____________________________________</p>  <br>  <figure class="image-pie"><img style="aspect-ratio: 1024/150;" src="../fotos/pieContrato.png?v=@DateTime.Now.Ticks" width="700" height="100"></figure>  </div>',
8, 1);



insert into TareaPlantilla (Descripcion, TipoTramiteId, Visibilidad, estado) values
('Firmar autorización', 6,1,0),
('Subir autorización firmada', 6,1,0),
('DNI del titular', 6,1,0),
('Firmar autorización', 8,1,0),
('Subir autorización firmada', 8,1,0),
('DNI del titular', 8,1,0),
('Firma autorización del titular', 2,1,0),
('DNI titular', 2,1,0),
('Abonar apertura', 2,1,0),
('Impuestos / deuda al día', 2,1,0),
('Asignar fecha al trámite', 2,1,0),
('Comprobar vínculo con titular', 2,1,0),
('Acta de defunción del fallecido', 2,1,0),
('Generar libre tránsito', 2,1,0),
('Firma autorización del titular', 5,1,0),
('DNI titular', 5,1,0),
('Abonar apertura', 5,1,0),
('Impuestos / deuda al día', 5,1,0),
('Asignar fecha al trámite', 5,1,0),
('Comprobar vínculo con titular', 5,1,0),
('Acta de defunción del fallecido', 5,1,0),
('Abonar cierre', 5,1,0),
('Firma autorización del titular', 3,1,0),
('DNI titular', 3,1,0),
('Abonar apertura', 3,1,0),
('Impuestos / deuda al día', 3,1,0),
('Asignar fecha al trámite', 3,1,0),
('Comprobar vínculo con titular', 3,1,0),
('Acta de defunción del fallecido', 3,1,0),
('Abonar cierre', 3,1,0),
('Abonar reducción', 3,1,0),
('Firmar autorización', 9,1,0),
('Subir autorización firmada', 9,1,0),
('DNI del titular', 9,1,0);





INSERT INTO RequisitosTramite (tipoTramiteId, descripcion)
VALUES 
(8, '- Se necesita que esté presente el nuevo titular.  - El trámite es sin costo.  - De lunes a viernes de 7:00hs a 12:30hs.'), --aceptacion de titularidad
(2, 'Para la cremación de {Difuntos} debe estar *al día con los impuestos* y *deuda*.   Debe abonar la apertura de la parcela en la municipalidad, que son *${precioApertura}* cada una.  Tiene que firmar el titular del nicho ({TitularesActuales}) y/o los familiares más cercanos al difunto.  Hay que comprobar el vínculo del fallecido con los firmantes, puede ser con libreta de familia, declaratoria de herederos o actas de nacimiento, esto depende de la relación.    La cremación tiene un costo de ${precioCremacion} por difunto, el cual lo abona en el Crematorio Parque los Álamos, aquí está incluido el traslado del cementerio municipal al crematorio.   Las autorizaciones las entrega esta oficina.'), --cremacion
(3, 'Para la reducción de {Difuntos} debe estar *al día con los impuestos* y *deuda*.   Debe abonar la apertura de la parcela en la municipalidad, que son *${precioApertura}* cada una.  Debe abonar el cierre de nicho o fosa. Cierre de nicho *${precioCierreNicho}* y cierre de fosa *${precioCierreFosa}*  La reducción tiene un costo de *${precioReduccion}*  Tiene que firmar el titular del nicho ({TitularesActuales}) y/o los familiares más cercanos al difunto.  Hay que comprobar el vínculo del fallecido con los firmantes, puede ser con libreta de familia, declaratoria de herederos o actas de nacimiento, esto depende de la relación.   Las autorizaciones las entrega esta oficina.'), --reduccion
(4, 'Contrato de concesion'),
(5, 'Para el traslado de {Difuntos} debe estar *al día con los impuestos* y *deuda*.   Debe abonar la apertura de la parcela en la municipalidad, que son *${precioApertura}* cada una.  Debe abonar el cierre de nicho o fosa, depende de donde sea trasladado. Cierre de nicho *${precioCierreNicho}* y cierre de fosa *${precioCierreFosa}*  Tiene que firmar el titular del nicho ({TitularesActuales}) y/o los familiares más cercanos al difunto.  Hay que comprobar el vínculo del fallecido con los firmantes, puede ser con libreta de familia, declaratoria de herederos o actas de nacimiento, esto depende de la relación.   Las autorizaciones las entrega esta oficina.'), --traslado
(6, '- Se necesita que esté presente el titular ({TitularesActuales}) y el nuevo titular.  - El trámite es sin costo.   - De lunes a viernes de 7:00hs a 12:30hs.'), --cambio de titular
(9, '- Se necesita que esté presente el titular ({TitularesActuales})   - El trámite es sin costo.     - De lunes a viernes de 7:00hs a 12:30hs.'); --permiso de ingreso

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
