<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Giriş Formu Kontrolü</title>
    <style>
        * { box-sizing: border-box; }
        body {
            font-family: Arial, sans-serif;
            background: #f4f6f8;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
        }
        .form-kutu {
            background: #fff;
            padding: 2rem;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 360px;
        }
        h1 {
            margin-top: 0;
            font-size: 1.4rem;
            text-align: center;
        }
        label {
            display: block;
            margin-bottom: 0.35rem;
            font-weight: bold;
        }
        input[type="text"],
        input[type="password"] {
            width: 100%;
            padding: 0.6rem;
            margin-bottom: 1rem;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
        button {
            width: 100%;
            padding: 0.7rem;
            background: #2563eb;
            color: #fff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 1rem;
        }
        button:hover { background: #1d4ed8; }
        .mesaj {
            margin-top: 1rem;
            padding: 0.75rem;
            border-radius: 4px;
            text-align: center;
            font-weight: bold;
        }
        .hata { background: #fee2e2; color: #b91c1c; }
    </style>
</head>
<body>
    <div class="form-kutu">
        <h1>Giriş Yap</h1>

        <form method="post" action="">
            <label for="kullanici">Kullanıcı Adı</label>
            <input type="text" id="kullanici" name="kullanici" required>

            <label for="sifre">Şifre</label>
            <input type="password" id="sifre" name="sifre" required>

            <button type="submit">Giriş</button>
        </form>

        <?php
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $kullanici = $_POST['kullanici'] ?? '';
            $sifre = $_POST['sifre'] ?? '';

            if ($kullanici === 'admin' && $sifre === '1234') {
                header('Location: basarili.php');
                exit;
            }

            echo '<div class="mesaj hata">Hatalı Bilgi</div>';
        }
        ?>
    </div>
</body>
</html>
