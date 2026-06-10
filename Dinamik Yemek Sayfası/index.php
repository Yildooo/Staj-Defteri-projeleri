<?php
require_once "config.php";

$sql = "SELECT id, baslik, aciklama, sure, porsiyon FROM tarifler ORDER BY id ASC";
$sonuc = $baglanti->query($sql);
?>
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Yemek Tarifleri</title>
    <link rel="stylesheet" href="style.css">
</head>
<body>
    <header class="site-header">
        <div class="container">
            <h1 class="logo">🍳 Yemek Tarifleri</h1>
            <p class="tagline">URL'deki ID'ye göre dinamik tarif sayfası</p>
        </div>
    </header>

    <main class="container">
        <section class="recipe-list">
            <?php if ($sonuc && $sonuc->num_rows > 0): ?>
                <?php while ($tarif = $sonuc->fetch_assoc()): ?>
                    <article class="recipe-card">
                        <h2>
                            <a href="tarif.php?id=<?= (int) $tarif["id"] ?>">
                                <?= htmlspecialchars($tarif["baslik"]) ?>
                            </a>
                        </h2>
                        <p><?= htmlspecialchars($tarif["aciklama"]) ?></p>
                        <div class="recipe-meta">
                            <span>⏱ <?= htmlspecialchars($tarif["sure"]) ?></span>
                            <span>🍽 <?= htmlspecialchars($tarif["porsiyon"]) ?></span>
                        </div>
                        <a href="tarif.php?id=<?= (int) $tarif["id"] ?>" class="btn">Tarifi Gör</a>
                    </article>
                <?php endwhile; ?>
            <?php else: ?>
                <p class="empty-message">Henüz tarif eklenmemiş. Önce <code>database.sql</code> dosyasını içe aktarın.</p>
            <?php endif; ?>
        </section>
    </main>

    <footer class="site-footer">
        <div class="container">
            <p>Örnek: <code>tarif.php?id=1</code></p>
        </div>
    </footer>
</body>
</html>
<?php $baglanti->close(); ?>
