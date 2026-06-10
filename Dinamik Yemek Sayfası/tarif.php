<?php
require_once "config.php";

$id = isset($_GET["id"]) ? (int) $_GET["id"] : 0;

if ($id <= 0) {
    header("Location: index.php");
    exit;
}

$sql = "SELECT * FROM tarifler WHERE id = $id";
$sonuc = $baglanti->query($sql);

if (!$sonuc || $sonuc->num_rows === 0) {
    http_response_code(404);
    $hata = "Bu ID ile eşleşen tarif bulunamadı.";
} else {
    $tarif = $sonuc->fetch_assoc();
}
?>
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?= isset($tarif) ? htmlspecialchars($tarif["baslik"]) : "Tarif Bulunamadı" ?> - Yemek Tarifleri</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <header class="site-header">
        <div class="container">
            <a href="index.php" class="logo">🍳 Yemek Tarifleri</a>
        </div>
    </header>

    <main class="container">
        <?php if (isset($hata)): ?>
            <section class="error-box">
                <h1>Tarif Bulunamadı</h1>
                <p><?= htmlspecialchars($hata) ?></p>
                <a href="index.php" class="btn">Tüm Tariflere Dön</a>
            </section>
        <?php else: ?>
            <article class="recipe-detail">
                <a href="index.php" class="back-link">← Tüm tarifler</a>
                <h1><?= htmlspecialchars($tarif["baslik"]) ?></h1>
                <p class="recipe-meta">
                    <span>⏱ <?= htmlspecialchars($tarif["sure"]) ?></span>
                    <span>🍽 <?= htmlspecialchars($tarif["porsiyon"]) ?></span>
                </p>
                <p class="recipe-desc"><?= htmlspecialchars($tarif["aciklama"]) ?></p>

                <div class="recipe-grid">
                    <section>
                        <h2>Malzemeler</h2>
                        <ul class="ingredient-list">
                            <?php foreach (explode("\n", $tarif["malzemeler"]) as $malzeme): ?>
                                <?php if (trim($malzeme) !== ""): ?>
                                    <li><?= htmlspecialchars(trim($malzeme)) ?></li>
                                <?php endif; ?>
                            <?php endforeach; ?>
                        </ul>
                    </section>
                    <section>
                        <h2>Hazırlanış</h2>
                        <ol class="steps-list">
                            <?php foreach (explode("\n", $tarif["hazirlanis"]) as $adim): ?>
                                <?php if (trim($adim) !== ""): ?>
                                    <li><?= htmlspecialchars(trim($adim)) ?></li>
                                <?php endif; ?>
                            <?php endforeach; ?>
                        </ol>
                    </section>
                </div>
            </article>
        <?php endif; ?>
    </main>

    <footer class="site-footer">
        <div class="container">
            <p>Dinamik Yemek Tarifi Sayfası &copy; <?= date("Y") ?></p>
        </div>
    </footer>
</body>
</html>
<?php $baglanti->close(); ?>
