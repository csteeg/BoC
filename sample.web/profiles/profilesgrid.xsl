<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xml:space="default" 
                xmlns:d="http://diggero.com/">
  <xsl:output method="html" omit-xml-declaration="yes" />
  <xsl:template match="/">
    <ul class="listofprofiles">
      <xsl:apply-templates select="/*[1]/d:Profile" />
    </ul>
  </xsl:template>

  <xsl:template match="d:Profile">
    <li class="profile">
      <span class="profile-voter">
        14<br />
        <span class="subtext">diggs</span><br />
        <a href="vote">lekker!</a>
      </span>
      <img class="profile-photo" src="#" alt="Photo" />
      <h4><xsl:value-of select="d:Nick" /></h4>
      <br />
      <xsl:value-of select="d:Gender" />,
      <xsl:value-of select="d:Age" /> jaar oud
      <br />
      <xsl:if test="d:CupSize/text() != ''">
        Cup <xsl:value-of select="d:CupSize" />, 
      </xsl:if>
      <xsl:value-of select="d:BodyType" />,
      <xsl:value-of select="d:Ethnicity" />
    </li>
  </xsl:template>
</xsl:stylesheet> 

