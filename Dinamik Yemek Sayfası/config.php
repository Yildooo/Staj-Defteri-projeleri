<?php
$host = "localhost";
$user = "root";
$pass = "";
$db   = "yemek_tarifleri";

$baglanti = new mysqli($host, $user, $pass, $db);

if ($baglanti->connect_error) {
    die("Veritabanı bağlantı hatası: " . $baglanti->connect_error);
}

$baglanti->set_charset("utf8mb4");
