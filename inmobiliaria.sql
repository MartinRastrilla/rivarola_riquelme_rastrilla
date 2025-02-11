-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 11-02-2025 a las 23:41:59
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `inquilino_dni` int(11) DEFAULT NULL,
  `inmueble_id` int(11) DEFAULT NULL,
  `estado` enum('Activo','Finalizado','Cancelado') DEFAULT NULL,
  `monto` decimal(10,2) DEFAULT NULL,
  `fecha_inicio` date DEFAULT NULL,
  `fecha_fin` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `inquilino_dni`, `inmueble_id`, `estado`, `monto`, `fecha_inicio`, `fecha_fin`) VALUES
(2, 12345601, 4, 'Activo', 230000.00, '2024-10-10', '2024-10-25'),
(5, 12345679, 2, 'Cancelado', 345000.00, '2025-01-26', '2025-02-26'),
(6, 12345678, 2, 'Cancelado', 600000.00, '2025-02-01', '2025-02-01'),
(10, 90909001, 4, 'Cancelado', 15000.00, '2025-02-03', '2025-02-21'),
(11, 33333333, 20, 'Cancelado', 0.00, '2025-02-04', '2024-02-04'),
(12, 43490178, 4, 'Activo', 15000.00, '2025-02-19', '2025-02-28'),
(16, 43490179, 21, 'Activo', 600000.00, '2025-02-12', '2025-02-28'),
(29, 12345601, 6, 'Cancelado', 10.00, '2025-02-12', '2025-02-14');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `direccion` varchar(255) DEFAULT NULL,
  `uso` enum('comercial','residencial') DEFAULT NULL,
  `tipo_id` int(11) DEFAULT NULL,
  `ambientes` int(11) DEFAULT NULL,
  `coordenadas` varchar(50) DEFAULT NULL,
  `precio` decimal(10,2) DEFAULT NULL,
  `propietario_dni` int(11) DEFAULT NULL,
  `estado` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `direccion`, `uso`, `tipo_id`, `ambientes`, `coordenadas`, `precio`, `propietario_dni`, `estado`) VALUES
(1, 'Av. Testing 122', 'residencial', 2, 4, 'none', 230000.00, 43490170, 1),
(2, 'Av. Vertientes 1455', 'residencial', 2, 45, 'none', 1900000.00, 43490170, 1),
(4, 'Av. Plazota 987', 'residencial', 2, 6, 'none', 420000.00, 12303111, 1),
(6, 'Av. Testing 211', 'residencial', 1, 4, 'none', 150000.00, 12202124, 1),
(20, 'Asa', 'comercial', 2, 1, 'none', 69.69, 23404908, 1),
(21, 'Las Weas 722', 'residencial', 3, 2, 'none', 25000.00, 12303111, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `dni` int(11) NOT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  `apellido` varchar(255) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `dni`, `nombre`, `apellido`, `telefono`, `email`) VALUES
(5, 12345601, 'Marta', 'Ramírez', '2665789256', 'a@a.com'),
(16, 654321, 'Rodrigo', 'Pérez', '2664998899', 'rodri@gmail.com'),
(17, 90909001, 'Paula', 'Perez', '266718705', 'pp@gmail.com'),
(32, 12345678, 'Gabi', 'Gome', '2265125588', 'a@a.com'),
(36, 12345679, 'Lukita', 'Leka', '2664198788', 'll@a.com'),
(37, 11111111, 'A', 'A', '1111111111', 'a@a.com'),
(39, 33333333, 'C', 'C', '3333333333', 'a@a.com'),
(43, 43490178, 'Martin', 'Rastrilla', '2664172839', 'mr@gmail.com'),
(44, 43490179, 'Laura', 'Lopez', '2664718293', 'll@a.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `multa`
--

CREATE TABLE `multa` (
  `id` int(11) NOT NULL,
  `contrato_id` int(11) NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `fecha_multa` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `multa`
--

INSERT INTO `multa` (`id`, `contrato_id`, `monto`, `fecha_multa`) VALUES
(7, 12, 30000.00, '2025-02-10'),
(9, 29, 20.00, '2025-02-11'),
(10, 6, 600000.00, '2025-02-11');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `contrato_id` int(11) NOT NULL,
  `fecha_pago` date NOT NULL,
  `detalle` varchar(255) NOT NULL,
  `importe` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`id`, `contrato_id`, `fecha_pago`, `detalle`, `importe`) VALUES
(3, 2, '2025-01-22', 'Algo', 1500.00),
(4, 2, '2025-01-23', 'Algo', 21.00),
(5, 2, '4620-05-22', 'Algo', 1.00),
(8, 5, '2025-02-05', 'C', 2000.00),
(9, 2, '2025-01-28', 'D', 1750.00),
(17, 2, '2025-01-29', 'Algo más corto', 920000.00),
(25, 5, '2025-10-05', 'Algo', 5000.00),
(26, 5, '2025-02-08', 'asd', 95000.00),
(27, 5, '2025-02-01', '1', 1500.00),
(28, 2, '2025-02-08', 'a', 1.00),
(29, 2, '2025-02-09', '2', 50.00),
(30, 5, '2025-03-01', 'pol', 555.00),
(31, 6, '2025-02-02', 'asd sad', 666.00);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `id` int(11) NOT NULL,
  `dni` int(11) NOT NULL,
  `apellido` varchar(255) DEFAULT NULL,
  `nombre` varchar(255) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`id`, `dni`, `apellido`, `nombre`, `telefono`, `email`) VALUES
(2, 43490170, 'Rastrilla', 'Martin', '2664778899', 'mr@gmail.com'),
(5, 12303111, 'Tobares', 'Fernando', '2667020202', 'ufa@gmail.com'),
(7, 23404908, 'Vázquez', 'Pilar', '2654998877', 'pv@gmail.com'),
(8, 12202124, 'Pagliani', 'Franco', '2667020201', 'fp@gmail.com'),
(11, 11111111, 'p1', 'p1', '1111111111', 'p1@p1.com'),
(12, 22222222, 'p2', 'p2', '2222222222', 'p2@p2.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `registro_contratos`
--

CREATE TABLE `registro_contratos` (
  `id` int(11) NOT NULL,
  `contrato_id` int(11) NOT NULL,
  `fecha_creacion` timestamp NOT NULL DEFAULT current_timestamp(),
  `creado_por` int(11) NOT NULL,
  `fecha_cancelacion` timestamp NULL DEFAULT NULL,
  `cancelado_por` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `registro_contratos`
--

INSERT INTO `registro_contratos` (`id`, `contrato_id`, `fecha_creacion`, `creado_por`, `fecha_cancelacion`, `cancelado_por`) VALUES
(12, 29, '2025-02-11 04:04:04', 17, '2025-02-11 21:04:06', 17);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `registro_pagos`
--

CREATE TABLE `registro_pagos` (
  `id` int(11) NOT NULL,
  `pagos_id` int(11) NOT NULL,
  `fecha_creacion` timestamp NOT NULL DEFAULT current_timestamp(),
  `creado_por` int(11) NOT NULL,
  `fecha_anulacion` timestamp NULL DEFAULT NULL,
  `anulado_por` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `roles`
--

CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `nombre` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `roles`
--

INSERT INTO `roles` (`id`, `nombre`) VALUES
(1, 'Administrador'),
(2, 'Empleado');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipos`
--

CREATE TABLE `tipos` (
  `id` int(11) NOT NULL,
  `nombre` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos`
--

INSERT INTO `tipos` (`id`, `nombre`) VALUES
(1, 'Casa'),
(2, 'Duplex'),
(3, 'Apartamento');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `id` int(11) NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `email` varchar(255) NOT NULL,
  `contrasenia` varchar(255) NOT NULL,
  `Rol` varchar(30) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`id`, `nombre`, `apellido`, `email`, `contrasenia`, `Rol`, `avatar`) VALUES
(13, 'Empleado', 'Empleado', 'empleado@gmail.com', 'KFzADE+l9G4eWlOER5sM5UnF2oVTzUE3uDZPL/T5s7Q=', 'Empleado', '/Uploads\\avatar_13.png'),
(14, 'Admin', 'Fort', 'admin@gmail.com', 'KFzADE+l9G4eWlOER5sM5UnF2oVTzUE3uDZPL/T5s7Q=', 'Administrador', '/Uploads\\avatar_14.jpg'),
(17, 'Martin', 'Rastrilla', 'rastrillamartin@gmail.com', 'qnVF4b4CeidwYvu24jXjLuUuMibwQCZRcYyMTKiTBQs=', 'Administrador', '/Uploads\\avatar_17.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios_roles`
--

CREATE TABLE `usuarios_roles` (
  `id` int(11) NOT NULL,
  `usuario_id` int(11) NOT NULL,
  `rol_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `inquilino_dni` (`inquilino_dni`),
  ADD KEY `inmueble_id` (`inmueble_id`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `propietario_dni` (`propietario_dni`),
  ADD KEY `tipo_id` (`tipo_id`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`) USING BTREE;

--
-- Indices de la tabla `multa`
--
ALTER TABLE `multa`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `contrato_id` (`contrato_id`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `dni` (`dni`) USING BTREE;

--
-- Indices de la tabla `registro_contratos`
--
ALTER TABLE `registro_contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `creado_por` (`creado_por`),
  ADD KEY `cancelado_por` (`cancelado_por`),
  ADD KEY `registro_contratos_ibfk_1` (`contrato_id`);

--
-- Indices de la tabla `registro_pagos`
--
ALTER TABLE `registro_pagos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `tipos`
--
ALTER TABLE `tipos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indices de la tabla `usuarios_roles`
--
ALTER TABLE `usuarios_roles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `rol_id` (`rol_id`),
  ADD KEY `usuario_id` (`usuario_id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=55;

--
-- AUTO_INCREMENT de la tabla `multa`
--
ALTER TABLE `multa`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `registro_contratos`
--
ALTER TABLE `registro_contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `registro_pagos`
--
ALTER TABLE `registro_pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `tipos`
--
ALTER TABLE `tipos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT de la tabla `usuarios_roles`
--
ALTER TABLE `usuarios_roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`inquilino_dni`) REFERENCES `inquilinos` (`dni`) ON UPDATE CASCADE,
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`inmueble_id`) REFERENCES `inmuebles` (`id`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`propietario_dni`) REFERENCES `propietarios` (`dni`) ON UPDATE CASCADE,
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`tipo_id`) REFERENCES `tipos` (`id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`contrato_id`) REFERENCES `contratos` (`id`);

--
-- Filtros para la tabla `registro_contratos`
--
ALTER TABLE `registro_contratos`
  ADD CONSTRAINT `registro_contratos_ibfk_1` FOREIGN KEY (`contrato_id`) REFERENCES `contratos` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `registro_contratos_ibfk_2` FOREIGN KEY (`creado_por`) REFERENCES `usuarios` (`id`),
  ADD CONSTRAINT `registro_contratos_ibfk_3` FOREIGN KEY (`cancelado_por`) REFERENCES `usuarios` (`id`);

--
-- Filtros para la tabla `usuarios_roles`
--
ALTER TABLE `usuarios_roles`
  ADD CONSTRAINT `usuarios_roles_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`),
  ADD CONSTRAINT `usuarios_roles_ibfk_2` FOREIGN KEY (`rol_id`) REFERENCES `roles` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
